using System.ComponentModel.DataAnnotations;

namespace remoview.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ✅ Moderasyon
        public ModerationStatus Status { get; set; } = ModerationStatus.Pending;
        public string? ModerationNote { get; set; }
        public int? ModeratedByUserId { get; set; }
        public DateTime? ModeratedAtUtc { get; set; }

        // Hangi filme, hangi kullanıcı tarafından yapıldı
        public int FilmId { get; set; }
        public int UserId { get; set; }

        // İlişkiler
        public virtual Film Film { get; set; }
        public virtual User User { get; set; }
    }
}
