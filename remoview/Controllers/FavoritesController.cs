using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using remoview.Data;
using remoview.Models;
using remoview.Dtos;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FavoritesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FavoritesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("{filmId}")]
    public async Task<IActionResult> AddFavorite(int filmId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var exists = await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.FilmId == filmId);

        if (exists)
            return BadRequest("Zaten favorilerde.");

        var favorite = new Favorite
        {
            UserId = userId,
            FilmId = filmId
        };

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // ✅ FilmSummaryDto dön: AverageRating + Genres dolu gelsin
    [HttpGet]
    public async Task<ActionResult<List<FilmSummaryDto>>> GetMyFavorites()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var favorites = await _context.Favorites
            .Where(f => f.UserId == userId)
            .Select(f => f.Film)
            .Select(film => new FilmSummaryDto
            {
                Id = film.Id,
                Title = film.Title,
                PosterUrl = film.PosterUrl,

                Genres = film.Genres.Select(g => g.Name).ToList(),

                AverageRating = film.Ratings.Any()
                    ? film.Ratings.Average(r => r.Value)
                    : 0.0
            })
            .ToListAsync();

        return Ok(favorites);
    }

    [HttpDelete("{filmId}")]
    public async Task<IActionResult> Remove(int filmId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var fav = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.FilmId == filmId);

        if (fav == null) return NotFound();

        _context.Favorites.Remove(fav);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
