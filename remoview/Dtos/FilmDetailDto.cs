using System.Collections.Generic;

namespace remoview.Dtos
{
    public class FilmDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? PosterUrl { get; set; }
        public double AverageRating { get; set; }
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();

        // ESKİSİ: public string? Genre { get; set; }
        // YENİSİ: Tür isimlerinin listesi
        public List<string> Genres { get; set; } = new List<string>();
    }
}