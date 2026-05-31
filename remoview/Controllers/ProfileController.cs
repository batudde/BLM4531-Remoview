using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using remoview.Data;
using remoview.Dtos;
using System.Security.Claims;

namespace remoview.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(ToProfileDto(user));
        }

        [HttpPut]
        public async Task<ActionResult<UserProfileDto>> UpdateProfile(UserProfileUpdateDto profileDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            user.ProfileDescription = profileDto.ProfileDescription?.Trim();
            await _context.SaveChangesAsync();

            return Ok(ToProfileDto(user));
        }

        private static UserProfileDto ToProfileDto(Models.User user)
        {
            return new UserProfileDto
            {
                Email = user.Email,
                Username = user.Username,
                ProfileDescription = user.ProfileDescription
            };
        }
    }
}
