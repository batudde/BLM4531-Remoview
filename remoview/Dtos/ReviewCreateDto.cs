using System.ComponentModel.DataAnnotations;

namespace remoview.Dtos
{
    // Bir filme yorum yapmak için kullanıcıdan bu bilgileri isteyeceğiz
    public class ReviewCreateDto
    {
        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; }
    }
}