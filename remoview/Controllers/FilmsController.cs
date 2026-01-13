using Microsoft.AspNetCore.Mvc;
using remoview.Data;
using remoview.Dtos;
using remoview.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization; // [Authorize] için eklendi
using System.Security.Claims; // Kullanıcı ID'sini token'dan almak için eklendi

namespace remoview.Controllers
{
    [Route("api/[controller]")] // Bu controller'a api/films adresinden erişilecek
    [ApiController]
    public class FilmsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FilmsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- YARDIMCI METOT ---
        // Token'dan o an giriş yapmış kullanıcının ID'sini alır
        private int GetUserIdFromToken()
        {
            // ClaimTypes.NameIdentifier, token içindeki user id
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new Exception("Kullanıcı kimliği token içinde bulunamadı.");
            }

            return int.Parse(userIdClaim.Value);
        }
        // ---------------------


        // GET: api/films
        // ✅ SADECE Approved filmleri listeler (Herkes erişebilir)
        [HttpGet]
        public async Task<IActionResult> GetAllFilms()
        {
            var films = await _context.Films
                .Where(f => f.Status == ModerationStatus.Approved) // ✅ EKLENDİ
                .Select(f => new FilmSummaryDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    PosterUrl = f.PosterUrl,
                    AverageRating = f.Ratings.Any() ? f.Ratings.Average(r => r.Value) : 0,
                    Genres = f.Genres.Select(g => g.Name).ToList()
                })
                .ToListAsync();

            return Ok(films);
        }

        // GET: api/films/5
        // ✅ Film Approved değilse dönmez
        // ✅ Reviews: sadece Approved yorumlar dönsün
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFilmById(int id)
        {
            var film = await _context.Films
                .Where(f => f.Id == id && f.Status == ModerationStatus.Approved) // ✅ EKLENDİ
                .Select(f => new FilmDetailDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    PosterUrl = f.PosterUrl,
                    AverageRating = f.Ratings.Any() ? f.Ratings.Average(r => r.Value) : 0,

                    Reviews = f.Reviews
                        .Where(r => r.Status == ModerationStatus.Approved) // ✅ EKLENDİ
                        .Select(r => new ReviewDto
                        {
                            Id = r.Id,
                            Comment = r.Comment,
                            CreatedAt = r.CreatedAt,
                            UserId = r.UserId
                        })
                        .ToList(),

                    Genres = f.Genres.Select(g => g.Name).ToList()
                })
                .FirstOrDefaultAsync();

            if (film == null)
            {
                return NotFound();
            }

            return Ok(film);
        }

        // POST: api/films
        // ✅ YENİ FİLM EKLER (Giriş yapmak ZORUNLU) -> Pending kaydolur
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateFilm(FilmCreateDto filmDto)
        {
            var userId = GetUserIdFromToken(); // ✅ EKLENDİ

            var film = new Film
            {
                Title = filmDto.Title?.Trim(),
                PosterUrl = filmDto.PosterUrl,

                // ✅ Moderasyon alanları
                Status = ModerationStatus.Pending,
                CreatedByUserId = userId,
                CreatedAtUtc = DateTime.UtcNow
            };

            // Gelen ID listesindeki Türleri veritabanından bul
            if (filmDto.GenreIds != null && filmDto.GenreIds.Any())
            {
                var genresFromDb = await _context.Genres
                    .Where(g => filmDto.GenreIds.Contains(g.Id))
                    .ToListAsync();

                film.Genres = genresFromDb;
            }

            _context.Films.Add(film);
            await _context.SaveChangesAsync();

            // FilmCreateResponseDto dönmeye devam (frontend kırılmasın)
            var createdFilmDto = new FilmCreateResponseDto
            {
                Id = film.Id,
                Title = film.Title,
                PosterUrl = film.PosterUrl,
                Genres = film.Genres.Select(g => g.Name).ToList()
            };

            // ✅ 201 + Location linki yerine 200 dönüyoruz (Pending film GetFilmById'da görünmeyecek)
            return Ok(createdFilmDto);
        }

        // POST: api/films/5/ratings
        // ✅ BİR FİLME PUAN VERİR (Giriş yapmak ZORUNLU) -> sadece Approved filme
        [HttpPost("{filmId}/ratings")]
        [Authorize]
        public async Task<IActionResult> AddRating(int filmId, RatingCreateDto ratingDto)
        {
            var userId = GetUserIdFromToken();

            // ✅ Film Approved mı?
            var filmApproved = await _context.Films
                .AnyAsync(f => f.Id == filmId && f.Status == ModerationStatus.Approved);

            if (!filmApproved)
            {
                return BadRequest("Film onaylı değil.");
            }

            // Bu kullanıcı bu filme daha önce puan vermiş mi?
            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.FilmId == filmId && r.UserId == userId);

            if (existingRating != null)
            {
                existingRating.Value = ratingDto.Value;
                _context.Ratings.Update(existingRating);
            }
            else
            {
                var rating = new Rating
                {
                    Value = ratingDto.Value,
                    FilmId = filmId,
                    UserId = userId
                };
                _context.Ratings.Add(rating);
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Puan başarıyla eklendi/güncellendi." });
        }

        // POST: api/films/5/reviews
        // ✅ BİR FİLME YORUM YAPAR (Giriş yapmak ZORUNLU) -> Pending kaydolur
        [HttpPost("{filmId}/reviews")]
        [Authorize]
        public async Task<IActionResult> AddReview(int filmId, ReviewCreateDto reviewDto)
        {
            var userId = GetUserIdFromToken();

            // ✅ Film Approved mı? (istersen kaldırırız ama mantıklısı bu)
            var filmApproved = await _context.Films
                .AnyAsync(f => f.Id == filmId && f.Status == ModerationStatus.Approved);

            if (!filmApproved)
            {
                return BadRequest("Film onaylı değil.");
            }

            var review = new Review
            {
                Comment = reviewDto.Comment?.Trim(),
                FilmId = filmId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,

                // ✅ Moderasyon
                Status = ModerationStatus.Pending
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Yorum onaya gönderildi." });
        }

        // YENİ METOT 1: DÜZENLEMEK İÇİN FİLM VERİSİ GETİRİR
        // GET: api/films/5/edit
        [HttpGet("{id}/edit")]
        [Authorize]
        public async Task<ActionResult<FilmEditDto>> GetFilmForEdit(int id)
        {
            var film = await _context.Films
                .Include(f => f.Genres)
                .Where(f => f.Id == id)
                .Select(f => new FilmEditDto
                {
                    Title = f.Title,
                    PosterUrl = f.PosterUrl,
                    SelectedGenreIds = f.Genres.Select(g => g.Id).ToList()
                })
                .FirstOrDefaultAsync();

            if (film == null)
            {
                return NotFound();
            }

            return Ok(film);
        }

        // YENİ METOT 2: FİLMİ GÜNCELLER
        // PUT: api/films/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateFilm(int id, FilmUpdateDto filmDto)
        {
            var filmToUpdate = await _context.Films
                .Include(f => f.Genres)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (filmToUpdate == null)
            {
                return NotFound();
            }

            // Basit alanları güncelle
            filmToUpdate.Title = filmDto.Title?.Trim();
            filmToUpdate.PosterUrl = filmDto.PosterUrl;

            // Türleri güncelle
            filmToUpdate.Genres.Clear();

            if (filmDto.GenreIds != null && filmDto.GenreIds.Any())
            {
                var newGenres = await _context.Genres
                    .Where(g => filmDto.GenreIds.Contains(g.Id))
                    .ToListAsync();

                filmToUpdate.Genres = newGenres;
            }

            // ⚠️ İstersen burada update sonrası tekrar Pending'e çekebiliriz:
            // filmToUpdate.Status = ModerationStatus.Pending;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Film başarıyla güncellendi." });
        }

        // GET: api/films/list
        // ✅ sızdırmamak için sadece Approved döndürelim
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<FilmListDto>>> GetFilmList()
        {
            var films = await _context.Films
                .Where(f => f.Status == ModerationStatus.Approved) // ✅ EKLENDİ
                .Include(f => f.Ratings)
                .Include(f => f.Genres)
                .ToListAsync();

            var result = films.Select(f => new FilmListDto
            {
                Id = f.Id,
                Title = f.Title,
                PosterUrl = f.PosterUrl,
                AverageRating = f.Ratings.Any()
                    ? f.Ratings.Average(r => r.Value)
                    : 0,
                Genres = f.Genres.Select(g => g.Name).ToList()
            }).ToList();

            return Ok(result);
        }
    }
}
