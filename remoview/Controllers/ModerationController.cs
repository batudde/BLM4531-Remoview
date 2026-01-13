using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using remoview.Data;
using remoview.Models;
using System.Security.Claims;

namespace remoview.Controllers
{
    [ApiController]
    [Route("api/moderation")]
    [Authorize] // giriş şart
    public class ModerationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ModerationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Token'dan email'i sağlam yakala (ClaimTypes.Email + schema url + "email")
        private string? GetEmailFromToken()
        {
            var email =
                User.FindFirstValue(ClaimTypes.Email) ??
                User.FindFirstValue("email") ??
                User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            return string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        }

        // ✅ Admin kontrolü: whitelist
        private bool IsSuperAdmin()
        {
            var email = GetEmailFromToken();
            if (email == null) return false;

            var superAdmins = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "batuhandede17@gmail.com",
                "deneme@gmail.com"
            };

            return superAdmins.Contains(email);
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub") ??
                User.FindFirstValue("id") ??
                User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (string.IsNullOrWhiteSpace(userIdClaim))
                throw new Exception("UserId claim bulunamadı.");

            return int.Parse(userIdClaim);
        }

        // ✅ DEBUG (SuperAdmin şartı yok, sadece authorize yeter)
        [HttpGet("debug")]
        public IActionResult DebugMe()
        {
            var email = GetEmailFromToken();

            var nameId =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier") ??
                User.FindFirstValue("sub") ??
                User.FindFirstValue("id");

            return Ok(new
            {
                email,
                nameId,
                isSuperAdmin = IsSuperAdmin(),
                claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            });
        }

        // ---------- FILMS ----------
        [HttpGet("films/pending")]
        public async Task<IActionResult> PendingFilms()
        {
            if (!IsSuperAdmin()) return Forbid();

            var list = await _context.Films
                .Where(f => f.Status == ModerationStatus.Pending)
                .OrderBy(f => f.CreatedAtUtc)
                .Select(f => new
                {
                    f.Id,
                    f.Title,
                    f.PosterUrl,
                    f.CreatedAtUtc,
                    f.CreatedByUserId
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost("films/{id:int}/approve")]
        public async Task<IActionResult> ApproveFilm(int id)
        {
            if (!IsSuperAdmin()) return Forbid();

            var film = await _context.Films.FirstOrDefaultAsync(f => f.Id == id);
            if (film == null) return NotFound();

            film.Status = ModerationStatus.Approved;
            film.ModeratedAtUtc = DateTime.UtcNow;
            film.ModeratedByUserId = GetUserIdFromToken();
            film.ModerationNote = null;

            await _context.SaveChangesAsync();
            return Ok();
        }

        public record RejectDto(string? Reason);

        [HttpPost("films/{id:int}/reject")]
        public async Task<IActionResult> RejectFilm(int id, [FromBody] RejectDto dto)
        {
            if (!IsSuperAdmin()) return Forbid();

            var film = await _context.Films.FirstOrDefaultAsync(f => f.Id == id);
            if (film == null) return NotFound();

            film.Status = ModerationStatus.Rejected;
            film.ModeratedAtUtc = DateTime.UtcNow;
            film.ModeratedByUserId = GetUserIdFromToken();
            film.ModerationNote = dto?.Reason?.Trim();

            await _context.SaveChangesAsync();
            return Ok();
        }

        // ---------- REVIEWS ----------
        [HttpGet("reviews/pending")]
        public async Task<IActionResult> PendingReviews()
        {
            if (!IsSuperAdmin()) return Forbid();

            var list = await _context.Reviews
                .Where(r => r.Status == ModerationStatus.Pending)
                .OrderBy(r => r.CreatedAt)
                .Select(r => new
                {
                    r.Id,
                    r.Comment,
                    r.CreatedAt,
                    r.UserId,
                    r.FilmId,
                    FilmTitle = r.Film.Title
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost("reviews/{id:int}/approve")]
        public async Task<IActionResult> ApproveReview(int id)
        {
            if (!IsSuperAdmin()) return Forbid();

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if (review == null) return NotFound();

            review.Status = ModerationStatus.Approved;
            review.ModeratedAtUtc = DateTime.UtcNow;
            review.ModeratedByUserId = GetUserIdFromToken();
            review.ModerationNote = null;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("reviews/{id:int}/reject")]
        public async Task<IActionResult> RejectReview(int id, [FromBody] RejectDto dto)
        {
            if (!IsSuperAdmin()) return Forbid();

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if (review == null) return NotFound();

            review.Status = ModerationStatus.Rejected;
            review.ModeratedAtUtc = DateTime.UtcNow;
            review.ModeratedByUserId = GetUserIdFromToken();
            review.ModerationNote = dto?.Reason?.Trim();

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
