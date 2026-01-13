using Microsoft.JSInterop; // IJSRuntime için
using System.Threading.Tasks;

namespace Remoview.Web.Services
{
    // Bu servis, jsinterop.js dosyasındaki JavaScript fonksiyonlarını çağırır
    public class LocalStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        // localStorage'a veri yaz
        public async Task SetItemAsync(string key, string value)
        {
            // JavaScript'teki localStorageFunctions.setItem fonksiyonunu çağır
            await _jsRuntime.InvokeVoidAsync("localStorageFunctions.setItem", key, value);
        }

        // localStorage'dan veri oku
        public async Task<string> GetItemAsync(string key)
        {
            // JavaScript'teki localStorageFunctions.getItem fonksiyonunu çağır
            return await _jsRuntime.InvokeAsync<string>("localStorageFunctions.getItem", key);
        }

        // localStorage'dan veri sil
        public async Task RemoveItemAsync(string key)
        {
            // JavaScript'teki localStorageFunctions.removeItem fonksiyonunu çağır
            await _jsRuntime.InvokeVoidAsync("localStorageFunctions.removeItem", key);
        }
    }
}