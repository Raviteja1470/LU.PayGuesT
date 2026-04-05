using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace PaYClient.LU.Services
{
    public class RefreshTokenService
    {
        private readonly ProtectedLocalStorage _protectedLocalStorage;
        private readonly string key = "refresh_token";
        public RefreshTokenService(ProtectedLocalStorage protectedLocalStorage)
        {
            _protectedLocalStorage = protectedLocalStorage;
        }

        public async Task set(string value)
        {
          await _protectedLocalStorage.SetAsync(key, value);
        }
        public async Task<string> get()
        {
            var result = await _protectedLocalStorage.GetAsync<string>(key);
            if (result.Success)
            return result.Value;
            return null;
        }
        internal async Task remove()
        {
            await _protectedLocalStorage.DeleteAsync(key);
        }
    }
}
