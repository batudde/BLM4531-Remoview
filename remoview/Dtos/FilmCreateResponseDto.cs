using System.Collections.Generic;

namespace remoview.Dtos
{
    // Film oluşturduktan sonra API'nin geri döndüreceği paket
    public class FilmCreateResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? PosterUrl { get; set; }
        public List<string> Genres { get; set; } = new List<string>(); // Metin listesi
    }
}