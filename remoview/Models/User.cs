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
        [MaxLength(40)]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string? ProfileDescription { get; set; }

        // İlişkiler: Bir kullanıcının birden fazla puanı ve yorumu olabilir
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Friendship> SentFriendRequests { get; set; } = new List<Friendship>();
        public virtual ICollection<Friendship> ReceivedFriendRequests { get; set; } = new List<Friendship>();
    }
}
