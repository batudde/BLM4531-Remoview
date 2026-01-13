using System.ComponentModel.DataAnnotations;

namespace remoview.Models
{
    public class Film
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } // Film İsmi

        public string? PosterUrl { get; set; } // Kapak Fotoğrafı (URL)

        // ✅ Moderasyon
        public ModerationStatus Status { get; set; } = ModerationStatus.Pending;
        public string? ModerationNote { get; set; }      // Reddetme sebebi vs
        public int? ModeratedByUserId { get; set; }
        public DateTime? ModeratedAtUtc { get; set; }

        public int? CreatedByUserId { get; set; }        // filmi kim ekledi
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        // İlişkiler: Bir filmin birden fazla puanı ve yorumu olabilir
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        // Bir filmin birden fazla türü olabilir
        public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}
