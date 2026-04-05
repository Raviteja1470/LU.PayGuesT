using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using PayingLu.Client.DTO;
using System.Text.Json.Serialization;

namespace PayingLu.Client.Services
{
    public class AuthService
    {
        private readonly AccessTokenService _accessTokenService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;
        private readonly NavigationManager _nav;
        public AuthService(AccessTokenService accessTokenService,
            ILogger<AuthService> logger, NavigationManager navigationManager,
            IHttpClientFactory httpClientFactory)
        {
            _accessTokenService = accessTokenService;
            _httpClient = httpClientFactory.CreateClient("PayingLuAPI");
            _logger = logger;
            _nav = navigationManager;
        }
        public async Task<bool> Login(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Auth/auth", new { email, password });

                if (response.IsSuccessStatusCode)
                {
                    // Cookie is set by API, nothing to store on client
                    return true;
                }

                _logger.LogWarning("Login failed: {StatusCode}", response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
            }

            return false;
        }

        //public async Task<bool> Login(string email, string password)
        //{
        //    try
        //    {
        //        var response = await _httpClient.PostAsJsonAsync("Auth/auth", new { email, password });
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var token = await response.Content.ReadAsStringAsync();
        //            var result = JsonConvert.DeserializeObject<AuthResponse>(token);
        //            await _accessTokenService.SetToken(result?.AccessToken);
        //            return true;
        //        }
        //        else
        //        {
        //            _logger.LogWarning("Login failed: {StatusCode}", response.StatusCode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error during login");
        //    }
        //    return false;


        //}
    }
}
