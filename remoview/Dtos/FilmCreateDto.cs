using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace remoview.Dtos
{
    public class FilmCreateDto
    {
        [Required]
        public string Title { get; set; }
        public string? PosterUrl { get; set; }

        // ESKİSİ: public string? Genre { get; set; }
        // YENİSİ: Tür ID'lerinin listesi (örn: [1, 3, 5])
        public List<int> GenreIds { get; set; } = new List<int>();
    }
}