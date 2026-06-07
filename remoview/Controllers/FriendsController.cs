using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using remoview.Data;
using remoview.Dtos;
using remoview.Models;
using System.Security.Claims;

namespace remoview.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FriendsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FriendsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<FriendUserDto>>> GetFriends()
        {
            var userId = GetCurrentUserId();
            var friendships = await _context.Friendships
                .Include(friendship => friendship.Requester)
                .Include(friendship => friendship.Addressee)
                .Where(friendship =>
                    friendship.Status == FriendshipStatus.Accepted &&
                    (friendship.RequesterId == userId || friendship.AddresseeId == userId))
                .OrderByDescending(friendship => friendship.RespondedAtUtc ?? friendship.CreatedAtUtc)
                .ToListAsync();

            var friends = new List<FriendUserDto>();

            foreach (var friendship in friendships)
            {
                var friend = friendship.RequesterId == userId ? friendship.Addressee : friendship.Requester;
                friends.Add(await ToFriendUserDto(friend));
            }

            return Ok(friends);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<FriendSearchResultDto>>> SearchUsers([FromQuery] string username)
        {
            var userId = GetCurrentUserId();
            var query = username.Trim();

            if (query.Length < 2)
            {
                return Ok(new List<FriendSearchResultDto>());
            }

            var users = await _context.Users
                .Where(user => user.Id != userId && EF.Functions.ILike(user.Username, $"%{query}%"))
                .OrderBy(user => user.Username)
                .Take(10)
                .ToListAsync();

            var results = new List<FriendSearchResultDto>();

            foreach (var user in users)
            {
                var relation = await GetFriendship(userId, user.Id);
                var userDto = await ToFriendUserDto(user);

                results.Add(new FriendSearchResultDto
                {
                    Id = userDto.Id,
                    Username = userDto.Username,
                    ProfileDescription = userDto.ProfileDescription,
                    FavoriteCount = userDto.FavoriteCount,
                    FriendCount = userDto.FriendCount,
                    FriendshipStatus = GetStatusText(relation, userId)
                });
            }

            return Ok(results);
        }

        [HttpPost("request/{username}")]
        public async Task<ActionResult> SendRequest(string username)
        {
            var userId = GetCurrentUserId();
            var normalizedUsername = username.Trim().ToLowerInvariant();
            var target = await _context.Users.FirstOrDefaultAsync(user => user.Username.ToLower() == normalizedUsername);

            if (target == null)
            {
                return NotFound("Kullanici bulunamadi.");
            }

            if (target.Id == userId)
            {
                return BadRequest("Kendine arkadas istegi gonderemezsin.");
            }

            var relation = await GetFriendship(userId, target.Id);

            if (relation != null && relation.Status == FriendshipStatus.Accepted)
            {
                return BadRequest("Bu kullanici zaten arkadas listende.");
            }

            if (relation != null && relation.Status == FriendshipStatus.Pending)
            {
                return BadRequest("Bu kullanici ile bekleyen bir arkadas istegi var.");
            }

            if (relation == null)
            {
                _context.Friendships.Add(new Friendship
                {
                    RequesterId = userId,
                    AddresseeId = target.Id,
                    Status = FriendshipStatus.Pending,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }
            else
            {
                relation.RequesterId = userId;
                relation.AddresseeId = target.Id;
                relation.Status = FriendshipStatus.Pending;
                relation.CreatedAtUtc = DateTime.UtcNow;
                relation.RespondedAtUtc = null;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("requests")]
        public async Task<ActionResult<List<FriendRequestDto>>> GetRequests()
        {
            var userId = GetCurrentUserId();
            var requests = await _context.Friendships
                .Include(friendship => friendship.Requester)
                .Where(friendship => friendship.AddresseeId == userId && friendship.Status == FriendshipStatus.Pending)
                .OrderByDescending(friendship => friendship.CreatedAtUtc)
                .ToListAsync();

            var response = new List<FriendRequestDto>();

            foreach (var request in requests)
            {
                response.Add(new FriendRequestDto
                {
                    Id = request.Id,
                    User = await ToFriendUserDto(request.Requester),
                    CreatedAtUtc = request.CreatedAtUtc
                });
            }

            return Ok(response);
        }

        [HttpPost("requests/{id}/accept")]
        public async Task<ActionResult> AcceptRequest(int id)
        {
            var userId = GetCurrentUserId();
            var request = await _context.Friendships.FirstOrDefaultAsync(friendship =>
                friendship.Id == id &&
                friendship.AddresseeId == userId &&
                friendship.Status == FriendshipStatus.Pending);

            if (request == null)
            {
                return NotFound("Arkadas istegi bulunamadi.");
            }

            request.Status = FriendshipStatus.Accepted;
            request.RespondedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("requests/{id}/reject")]
        public async Task<ActionResult> RejectRequest(int id)
        {
            var userId = GetCurrentUserId();
            var request = await _context.Friendships.FirstOrDefaultAsync(friendship =>
                friendship.Id == id &&
                friendship.AddresseeId == userId &&
                friendship.Status == FriendshipStatus.Pending);

            if (request == null)
            {
                return NotFound("Arkadas istegi bulunamadi.");
            }

            request.Status = FriendshipStatus.Rejected;
            request.RespondedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("profile/{username}")]
        public async Task<ActionResult<FriendUserDto>> GetPublicProfile(string username)
        {
            var normalizedUsername = username.Trim().ToLowerInvariant();
            var user = await _context.Users.FirstOrDefaultAsync(target => target.Username.ToLower() == normalizedUsername);

            if (user == null)
            {
                return NotFound("Kullanici bulunamadi.");
            }

            return Ok(await ToFriendUserDto(user, includeFavoriteFilms: true));
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private async Task<Friendship?> GetFriendship(int userId, int targetUserId)
        {
            return await _context.Friendships.FirstOrDefaultAsync(friendship =>
                (friendship.RequesterId == userId && friendship.AddresseeId == targetUserId) ||
                (friendship.RequesterId == targetUserId && friendship.AddresseeId == userId));
        }

        private async Task<FriendUserDto> ToFriendUserDto(User user, bool includeFavoriteFilms = false)
        {
            var favoriteCount = await _context.Favorites.CountAsync(favorite => favorite.UserId == user.Id);
            var friendCount = await _context.Friendships.CountAsync(friendship =>
                friendship.Status == FriendshipStatus.Accepted &&
                (friendship.RequesterId == user.Id || friendship.AddresseeId == user.Id));
            var favoriteFilms = includeFavoriteFilms
                ? await _context.Favorites
                    .Where(favorite => favorite.UserId == user.Id)
                    .Include(favorite => favorite.Film)
                        .ThenInclude(film => film.Genres)
                    .Include(favorite => favorite.Film)
                        .ThenInclude(film => film.Ratings)
                    .OrderByDescending(favorite => favorite.CreatedAt)
                    .Select(favorite => new FilmListDto
                    {
                        Id = favorite.Film.Id,
                        Title = favorite.Film.Title,
                        PosterUrl = favorite.Film.PosterUrl,
                        AverageRating = favorite.Film.Ratings.Any()
                            ? favorite.Film.Ratings.Average(rating => rating.Value)
                            : 0,
                        Genres = favorite.Film.Genres.Select(genre => genre.Name).ToList()
                    })
                    .ToListAsync()
                : new List<FilmListDto>();

            return new FriendUserDto
            {
                Id = user.Id,
                Username = user.Username,
                ProfileDescription = user.ProfileDescription,
                FavoriteCount = favoriteCount,
                FriendCount = friendCount,
                FavoriteFilms = favoriteFilms
            };
        }

        private static string GetStatusText(Friendship? friendship, int userId)
        {
            if (friendship == null)
            {
                return "none";
            }

            if (friendship.Status == FriendshipStatus.Accepted)
            {
                return "friends";
            }

            if (friendship.Status == FriendshipStatus.Rejected)
            {
                return "none";
            }

            return friendship.RequesterId == userId ? "pending_sent" : "pending_received";
        }
    }
}
