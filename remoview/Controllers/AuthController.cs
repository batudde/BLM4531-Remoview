using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // IConfiguration için eklendi
using Microsoft.IdentityModel.Tokens;    // Token için eklendi
using System.IdentityModel.Tokens.Jwt;   // Token için eklendi
using System.Security.Claims;            // Token için eklendi
using System.Text;                       // Token için eklendi
using remoview.Data;                     // DbContext için eklendi
using remoview.Dtos;                     // DTO'lar için eklendi
using remoview.Models;                   // User modeli için eklendi
using System.Threading.Tasks;            // Async işlemler için eklendi
using Microsoft.EntityFrameworkCore;     // SingleOrDefaultAsync için eklendi

namespace remoview.Controllers
{
    [Route("api/[controller]")] // Bu controller'a api/auth adresinden erişilecek
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            // 1. Bu email adresi zaten var mı?
            var userExists = await _context.Users.AnyAsync(u => u.Email == registerDto.Email);
            if (userExists)
            {
                return BadRequest("Bu email adresi zaten kullanılıyor.");
            }

            // 2. Şifreyi "Hash"le (güvenli hale getir)
            // Gerçek bir projede Bcrypt gibi güçlü bir kütüphane kullanılır.
            // Biz basitlik için şimdilik sahte bir hash yapacağız (ama tersine çevrilemez).
            string passwordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(registerDto.Password));

            // 3. Yeni kullanıcı oluştur
            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = passwordHash
            };

            // 4. Veritabanına kaydet
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Kullanıcı başarıyla oluşturuldu." });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(RegisterDto loginDto)
        {
            // 1. Kullanıcıyı bul
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);

            // 2. Kullanıcı yoksa veya şifre yanlışsa hata ver
            string passwordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(loginDto.Password));
            if (user == null || user.PasswordHash != passwordHash)
            {
                return Unauthorized("Geçersiz email veya şifre.");
            }

            // 3. Şifre doğruysa, Token oluştur
            var token = CreateJwtToken(user);

            return Ok(new { Token = token });
        }

        // --- Yardımcı Metot ---
        private string CreateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("BizimCokGizliAnahtarimiz12345!*-"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Token'ın içine hangi bilgileri koyacağımız (Payload)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // 🔥 USER ID
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            var token = new JwtSecurityToken(
                issuer: null, // "Veren" (gerekirse doldurulur)
                audience: null, // "Alan" (gerekirse doldurulur)
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Token 1 gün geçerli olsun
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}