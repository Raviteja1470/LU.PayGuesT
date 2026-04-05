using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace PaYClient.LU.Services
{
    public class APIService
    {
        private readonly HttpClient client;
        private readonly AccessTokenService tokenService;
        private readonly AuthService authService;
        private readonly NavigationManager navigationManager;

        public APIService(
            IHttpClientFactory httpClientFactory,
            AccessTokenService accessTokenService,
            AuthService authService,
            NavigationManager navigationManager)
        {
            client = httpClientFactory.CreateClient("ApiClient");
            this.tokenService = accessTokenService;
            this.authService = authService;
            this.navigationManager = navigationManager;
        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            // Get current access token
            var token = await tokenService.GetToken();

            // Attach token to request
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Make API call
            var responseMessage = await client.GetAsync(endpoint);

            // If token expired or invalid
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Try refresh token
                var refreshTokenResult = await authService.RefreshTokenAsync();

                if (!refreshTokenResult)
                {
                    // Refresh failed → logout user
                    await authService.Logout();
                    return responseMessage;
                }

                // Refresh succeeded → get new token
                var newToken = await tokenService.GetToken();

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", newToken);

                // Retry the original request
                var newResponse = await client.GetAsync(endpoint);
                return newResponse;
            }

            return responseMessage;
        }

        public async Task<HttpResponseMessage> PostDataAsync(string endpoint, object obj)
        {
            // Get current access token
            var token = await tokenService.GetToken();

            // Attach token to request
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Make API call
            var responseMessage = await client.PostAsJsonAsync(endpoint,obj);

            // If token expired or invalid
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Try refresh token
                var refreshTokenResult = await authService.RefreshTokenAsync();

                if (!refreshTokenResult)
                {
                    // Refresh failed → logout user
                    await authService.Logout();
                    return responseMessage;
                }

                // Refresh succeeded → get new token
                var newToken = await tokenService.GetToken();

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", newToken);

                // Retry the original request
                var newResponse = await client.GetAsync(endpoint);
                return newResponse;
            }

            return responseMessage;
        }
    }
    
}
