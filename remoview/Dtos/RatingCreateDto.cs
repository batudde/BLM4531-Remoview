using System.ComponentModel.DataAnnotations;

namespace remoview.Dtos
{
    // Bir filme puan vermek için kullanıcıdan bu bilgileri isteyeceğiz
    public class RatingCreateDto
    {
        [Required]
        [Range(1, 5)]
        public int Value { get; set; } // 1-5 arası puan
    }
}