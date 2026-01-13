using System.Collections.Generic;

namespace remoview.Dtos
{
    // Bir filmi düzenlemek için forma göndereceğimiz veriler
    public class FilmEditDto
    {
        public string Title { get; set; }
        public string? PosterUrl { get; set; }

        // Filmin şu anda sahip olduğu Türlerin ID'leri
        public List<int> SelectedGenreIds { get; set; } = new List<int>();
    }
}