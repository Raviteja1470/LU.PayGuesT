using Microsoft.AspNetCore.Components.Authorization;
using PaYClient.LU.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PaYClient.LU.Security
{
    public class JWTAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AccessTokenService accessTokenService;

        public JWTAuthenticationStateProvider(AccessTokenService accessTokenService)
        {
            this.accessTokenService = accessTokenService;
        }

        //public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        //{
        //    try
        //    {
        //        var token = await accessTokenService.GetToken();

        //        if (string.IsNullOrWhiteSpace(token))
        //            return await MarkAsUnauthorize();

        //        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        //        var identity = new ClaimsIdentity(jwt.Claims, "JWT");
        //        var principal = new ClaimsPrincipal(identity);

        //        return new AuthenticationState(principal);
        //    }
        //    catch
        //    {
        //        return await MarkAsUnauthorize();
        //    }

        //}
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await accessTokenService.GetToken();

            if (string.IsNullOrEmpty(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var identity = new ClaimsIdentity(jwt.Claims, "JWT");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        public async Task<AuthenticationState> MarkAsUnauthorize()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            var state = new AuthenticationState(anonymous);

            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var identity = new ClaimsIdentity(jwt.Claims, "JWT");
            var principal = new ClaimsPrincipal(identity);

            var state = new AuthenticationState(principal);

            NotifyAuthenticationStateChanged(Task.FromResult(state));
        }
    }

}
