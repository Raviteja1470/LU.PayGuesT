using Microsoft.JSInterop;

namespace PayingLu.Client.Services
{
    public class CookieService
    {
        private readonly IJSRuntime _runtime;

        public CookieService(IJSRuntime runtime)
        {
            _runtime = runtime;
        }

        public async Task<string> GetCookie(string key)
        {
            return await _runtime.InvokeAsync<string>("getCookie", key);
        }
        public async Task<bool> removeCookie(string key)
        {
            await _runtime.InvokeVoidAsync("deleteCookie", key);
            return true;
        }
        public async Task<bool> SetCookie(string key, string value, int days)
        {
            await _runtime.InvokeVoidAsync("setCookie", key, value, days);
            return true;
        }
    }
}
