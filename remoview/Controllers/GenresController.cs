using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using remoview.Data;
using remoview.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace remoview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/genres
        // Tüm türleri listeler (örn: [ { "id": 1, "name": "Action" }, ... ])
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
        {
            // Şimdilik, veritabanına birkaç varsayılan tür ekleyelim (eğer boşsa)
            if (!_context.Genres.Any())
            {
                _context.Genres.AddRange(
                    new Genre { Name = "Action" },
                    new Genre { Name = "Comedy" },
                    new Genre { Name = "Sci-Fi" },
                    new Genre { Name = "Drama" },
                    new Genre { Name = "Horror" }
                );
                await _context.SaveChangesAsync();
            }

            return await _context.Genres.OrderBy(g => g.Name).ToListAsync();
        }
    }
}