using Microsoft.EntityFrameworkCore;
using remoview.Data;
using remoview.Models;

namespace remoview.Data
{
    public static class DbSeeder
    {
        public static async Task SeedGenresAsync(ApplicationDbContext context)
        {
            // DB hazır değilse bile patlamasın
            await context.Database.MigrateAsync();

            // Eğer zaten doluysa tekrar ekleme (istersen bu kontrolü kaldır)
            // if (await context.Genres.AnyAsync()) return;

            // 30-40 tane geniş liste
            var genreNames = new[]
            {
                "Action", "Adventure", "Animation", "Biography", "Comedy", "Crime", "Documentary", "Drama",
                "Family", "Fantasy", "Film-Noir", "History", "Horror", "Music", "Musical", "Mystery",
                "Romance", "Sci-Fi", "Sport", "Thriller", "War", "Western",

                "Superhero", "Psychological Thriller", "Slasher", "Zombie", "Vampire",
                "Coming-of-Age", "Teen", "Satire", "Dark Comedy",
                "Detective", "Courtroom", "Heist", "Gangster",
                "Cyberpunk", "Steampunk", "Space Opera", "Dystopian", "Post-Apocalyptic",
                "Martial Arts", "Spy", "Military", "Disaster", "Survival",
                "Anime", "Live Action", "Mockumentary"
            };

            // Case-insensitive duplicate engeli
            var existing = await context.Genres
                .Select(g => g.Name)
                .ToListAsync();

            var existingSet = new HashSet<string>(existing, StringComparer.OrdinalIgnoreCase);

            var toAdd = genreNames
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n.Trim())
                .Where(n => !existingSet.Contains(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(n => new Genre { Name = n })
                .ToList();

            if (toAdd.Count == 0) return;

            context.Genres.AddRange(toAdd);
            await context.SaveChangesAsync();
        }
    }
}
