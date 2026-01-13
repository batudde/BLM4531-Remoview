using System.Collections.Generic;

namespace remoview.Dtos
{
    public class FilmListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? PosterUrl { get; set; }
        public double AverageRating { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
    }
}
