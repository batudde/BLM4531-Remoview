using System.ComponentModel.DataAnnotations;

namespace remoview.Dtos
{
    public class RegisterDto
    {
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
