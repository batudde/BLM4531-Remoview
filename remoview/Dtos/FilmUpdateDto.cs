using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace remoview.Dtos
{
    // Filmi güncellemek için API'ye göndereceğimiz veriler
    public class FilmUpdateDto
    {
        [Required]
        public string Title { get; set; }
        public string? PosterUrl { get; set; }

        // Kullanıcının seçtiği YENİ Tür ID'lerinin listesi
        public List<int> GenreIds { get; set; } = new List<int>();
    }
}