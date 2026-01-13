using remoview.Dtos;
using remoview.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Http;

namespace Remoview.Web.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly LocalStorageService _localStorageService;

        public ApiClient(IHttpClientFactory httpClientFactory, LocalStorageService localStorageService)
        {
            _httpClient = httpClientFactory.CreateClient("RemoviewApi");
            _localStorageService = localStorageService;
        }

        // --- GÜVENLİK (AUTH) ---
        private async Task EnsureAuthHeaderAsync()
        {
            var token = await _localStorageService.GetItemAsync("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                return;
            }

            var current = _httpClient.DefaultRequestHeaders.Authorization?.Parameter;
            if (current != token)
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // --- AUTH ---
        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", registerDto);
            if (response.IsSuccessStatusCode)
            {
                var successResult = await response.Content.ReadFromJsonAsync<ApiMessageResponse>();
                return successResult?.Message ?? "Kayıt başarılı!";
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(errorContent) && errorContent.Contains("Bu email adresi zaten kullanılıyor"))
            {
                return "Hata: Bu email adresi zaten kullanılıyor.";
            }
            return $"Hata: {errorContent}";
        }

        public async Task<string?> LoginAsync(RegisterDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginDto);
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (string.IsNullOrWhiteSpace(result?.Token)) return null;

            await _localStorageService.SetItemAsync("authToken", result.Token);

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", result.Token);

            return result.Token;
        }

        public async Task LogoutAsync()
        {
            await _localStorageService.RemoveItemAsync("authToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        // --- GENRES / FILMS ---
        public async Task<List<Genre>> GetGenresAsync()
        {
            var response = await _httpClient.GetAsync("/api/genres");
            if (response.IsSuccessStatusCode)
            {
                var genres = await response.Content.ReadFromJsonAsync<List<Genre>>();
                return genres ?? new List<Genre>();
            }
            return new List<Genre>();
        }

        public async Task<List<FilmSummaryDto>> GetFilmsAsync()
        {
            var response = await _httpClient.GetAsync("/api/films");
            if (response.IsSuccessStatusCode)
            {
                var films = await response.Content.ReadFromJsonAsync<List<FilmSummaryDto>>();
                return films ?? new List<FilmSummaryDto>();
            }
            return new List<FilmSummaryDto>();
        }

        public async Task<FilmDetailDto?> GetFilmDetailAsync(int filmId)
        {
            var response = await _httpClient.GetAsync($"/api/films/{filmId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<FilmDetailDto>();
            }
            return null;
        }

        public async Task<FilmCreateResponseDto?> CreateFilmAsync(FilmCreateDto filmDto)
        {
            await EnsureAuthHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("/api/films", filmDto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<FilmCreateResponseDto>();
            }
            return null;
        }

        public async Task<bool> AddRatingAsync(int filmId, RatingCreateDto ratingDto)
        {
            await EnsureAuthHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync($"/api/films/{filmId}/ratings", ratingDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddReviewAsync(int filmId, ReviewCreateDto reviewDto)
        {
            await EnsureAuthHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync($"/api/films/{filmId}/reviews", reviewDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<FilmEditDto?> GetFilmForEditAsync(int filmId)
        {
            await EnsureAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"/api/films/{filmId}/edit");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<FilmEditDto>();
            }
            return null;
        }

        public async Task<bool> UpdateFilmAsync(int filmId, FilmUpdateDto filmDto)
        {
            await EnsureAuthHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync($"/api/films/{filmId}", filmDto);
            return response.IsSuccessStatusCode;
        }

        // ✅ FAVORITES
        public async Task<List<FilmSummaryDto>> GetFavoritesAsync()
        {
            await EnsureAuthHeaderAsync();
            var response = await _httpClient.GetAsync("/api/Favorites");
            if (response.IsSuccessStatusCode)
            {
                var favs = await response.Content.ReadFromJsonAsync<List<FilmSummaryDto>>();
                return favs ?? new List<FilmSummaryDto>();
            }
            return new List<FilmSummaryDto>();
        }

        public async Task<bool> AddFavoriteAsync(int filmId)
        {
            await EnsureAuthHeaderAsync();
            var response = await _httpClient.PostAsync($"/api/Favorites/{filmId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveFavoriteAsync(int filmId)
        {
            await EnsureAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync($"/api/Favorites/{filmId}");
            return response.IsSuccessStatusCode;
        }

        // ✅ MODERATION DEBUG
        public async Task<string> GetModerationDebugAsync()
        {
            await EnsureAuthHeaderAsync();
            var res = await _httpClient.GetAsync("/api/moderation/debug");
            return await res.Content.ReadAsStringAsync();
        }

        // ✅ MODERATION (SuperAdmin)
        public async Task<List<PendingFilmDto>> GetPendingFilmsAsync()
        {
            await EnsureAuthHeaderAsync();
            var res = await _httpClient.GetAsync("/api/moderation/films/pending");
            if (!res.IsSuccessStatusCode) return new List<PendingFilmDto>();
            return await res.Content.ReadFromJsonAsync<List<PendingFilmDto>>() ?? new List<PendingFilmDto>();
        }

        public async Task<List<PendingReviewDto>> GetPendingReviewsAsync()
        {
            await EnsureAuthHeaderAsync();
            var res = await _httpClient.GetAsync("/api/moderation/reviews/pending");
            if (!res.IsSuccessStatusCode) return new List<PendingReviewDto>();
            return await res.Content.ReadFromJsonAsync<List<PendingReviewDto>>() ?? new List<PendingReviewDto>();
        }

        public async Task<bool> ApproveFilmAsync(int id)
        {
            await EnsureAuthHeaderAsync();
            var res = await _httpClient.PostAsync($"/api/moderation/films/{id}/approve", null);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> RejectFilmAsync(int id, string reason)
        {
            await EnsureAuthHeaderAsync();
            var res = await _httpClient.PostAsJsonAsync($"/api/moderation/films/{id}/reject", new { reason });
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> ApproveReviewAsync(int id)
        {
            await EnsureAuthHeaderAsync();
            var res = await _httpClient.PostAsync($"/api/moderation/reviews/{id}/approve", null);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> RejectReviewAsync(int id, string reason)
        {
            await EnsureAuthHeaderAsync();
            var res = await _httpClient.PostAsJsonAsync($"/api/moderation/reviews/{id}/reject", new { reason });
            return res.IsSuccessStatusCode;
        }

        // DTOs
        public class PendingFilmDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string? PosterUrl { get; set; }
            public DateTime CreatedAtUtc { get; set; }
            public int? CreatedByUserId { get; set; }
        }

        public class PendingReviewDto
        {
            public int Id { get; set; }
            public string Comment { get; set; } = "";
            public DateTime CreatedAt { get; set; }
            public int UserId { get; set; }
            public int FilmId { get; set; }
            public string FilmTitle { get; set; } = "";
        }

        private class ApiMessageResponse
        {
            public string Message { get; set; } = "";
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = "";
    }
}
