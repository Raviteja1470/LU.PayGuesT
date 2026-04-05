using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using PaYClient.LU.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Net.Http;

namespace PaYClient.LU.Services
{
    public class AuthService
    {
        private readonly AccessTokenService accessTokenService;
        private readonly NavigationManager nav;
        private readonly HttpClient client;
        private readonly CustomAuthenticationStateProvider authStateProvider;
        private readonly RefreshTokenService _refreshTokenService;

        public AuthService(
            AccessTokenService accessTokenService,
            NavigationManager nav,
            IHttpClientFactory httpClientFactory,
            RefreshTokenService refreshTokenService,
            CustomAuthenticationStateProvider authStateProvider)
        {
            this.accessTokenService = accessTokenService;
            this.nav = nav;
            this.authStateProvider = authStateProvider;
            client = httpClientFactory.CreateClient("ApiClient");
            _refreshTokenService = refreshTokenService;
        }

        public async Task<bool> Login(string email, string password)
        {
            var status = await client.PostAsJsonAsync("Auth/auth", new { email, password });

            if (status.IsSuccessStatusCode)
            {
                var token = await status.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<AuthResponse>(token);

                await accessTokenService.SetToken(result.AccessToken);
                await _refreshTokenService.set(result.refreshToken);


                // 🔥 Tell Blazor the user is authenticated
                await MarkUserAsAuthenticated(result.AccessToken);

                return true;
            }

            return false;
        }
        public async Task<bool> RefreshTokenAsync()
        {
            // Get refresh token from storage
            var refreshToken = await _refreshTokenService.get();

            // Attach refresh token as cookie header
            client.DefaultRequestHeaders.Add("Cookie", $"refreshtoken={refreshToken}");

            // Call API endpoint
            var responseMessage = await client.PostAsync("auth/refresh", null);

            if (responseMessage.IsSuccessStatusCode)
            {
                var token = await responseMessage.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(token))
                {
                    var result = JsonConvert.DeserializeObject<AuthResponse>(token);

                    // Save new access token
                    await accessTokenService.SetToken(result.AccessToken);

                    // Save new refresh token
                    await _refreshTokenService.set(result.refreshToken);

                    return true;
                }
            }

            return false;
        }

        public async Task Logout() 
        {
            var refreshToken = await _refreshTokenService.get();
            client.DefaultRequestHeaders.Add("Cookie", $"refreshtoken= {refreshToken}");
            var responsemessage = await client.PostAsync("Auth/logout", null);
            if (responsemessage.IsSuccessStatusCode)
            {
                await accessTokenService.RemoveToken();
                await _refreshTokenService.remove();
                nav.NavigateTo("/login", forceLoad: true);

            }


        //{
        //    await accessTokenService.RemoveToken();
        //    // 🔥 Tell Blazor the user is now anonymous
        //    authStateProvider.NotifyAuthenticationStateChangedPublic(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
        //    nav.NavigateTo("/login");
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var identity = new ClaimsIdentity(jwt.Claims, "JWT");
            var principal = new ClaimsPrincipal(identity);

            var state = new AuthenticationState(principal);

            // 🔥 This tells Blazor: "User is now authenticated"
            authStateProvider.NotifyAuthenticationStateChangedPublic(Task.FromResult(state));
        }
        //public async Task<bool> Login(string email, string password)
        //{
        //    var status = await client.PostAsJsonAsync("Auth/auth", new { email, password });

        //    if (status.IsSuccessStatusCode)
        //    {
        //        var token = await status.Content.ReadAsStringAsync();
        //        var result = JsonConvert.DeserializeObject<AuthResponse>(token);

        //        await accessTokenService.SetToken(result.AccessToken);

        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }

    // Minimal AuthenticationStateProvider subclass that exposes the protected NotifyAuthenticationStateChanged method.
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        public void NotifyAuthenticationStateChangedPublic(Task<AuthenticationState> task)
        {
            // calls the protected method from the base class
            NotifyAuthenticationStateChanged(task);
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Default to anonymous user. Real implementation can read stored token and build ClaimsPrincipal.
            var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            return Task.FromResult(anonymous);
        }

        
    }


}

