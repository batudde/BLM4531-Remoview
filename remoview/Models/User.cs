using System.ComponentModel.DataAnnotations;

namespace remoview.Models // Proje adınız "remoview" ise bu şekilde olmalı
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }

        // İlişkiler: Bir kullanıcının birden fazla puanı ve yorumu olabilir
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}