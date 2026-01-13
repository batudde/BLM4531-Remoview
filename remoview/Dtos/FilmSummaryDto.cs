using System.Collections.Generic;

namespace remoview.Dtos
{
    public class FilmSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? PosterUrl { get; set; }
        public double AverageRating { get; set; }

        // ESKİSİ: public string? Genre { get; set; }
        // YENİSİ: Tür isimlerinin listesi (örn: ["Action", "Sci-Fi"])
        public List<string> Genres { get; set; } = new List<string>();
    }
}