using System.ComponentModel.DataAnnotations;

namespace remoview.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 5)] // Puanın 1 ile 5 arasında olmasını zorunlu kılar
        public int Value { get; set; }

        // Hangi filme, hangi kullanıcı tarafından verildi
        public int FilmId { get; set; }
        public int UserId { get; set; }

        // İlişkiler
        public virtual Film Film { get; set; }
        public virtual User User { get; set; }
    }
}