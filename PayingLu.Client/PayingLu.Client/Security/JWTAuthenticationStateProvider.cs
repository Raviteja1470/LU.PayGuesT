using Microsoft.AspNetCore.Components.Authorization;
using PayingLu.Client.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PayingLu.Client.Security
{
    public class JWTAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JWTAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User
                       ?? new ClaimsPrincipal(new ClaimsIdentity());

            return Task.FromResult(new AuthenticationState(user));
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }
    }


    //public class JWTAuthenticationStateProvider : AuthenticationStateProvider
    //{
    //    private readonly IHttpContextAccessor _httpContextAccessor;

    //    public JWTAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
    //    {
    //        _httpContextAccessor = httpContextAccessor;
    //    }

    //    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    //    {
    //        var user = _httpContextAccessor.HttpContext?.User
    //                   ?? new ClaimsPrincipal(new ClaimsIdentity());

    //        return Task.FromResult(new AuthenticationState(user));
    //    }

    //    public void MarkUserAsLoggedOut()
    //    {
    //        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
    //        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    //    }
    //}


    //public class JWTAuthenticationStateProvider : AuthenticationStateProvider
    //{
    //    private readonly AccessTokenService _accessTokenService;

    //    public JWTAuthenticationStateProvider(AccessTokenService accessTokenService)
    //    {
    //        _accessTokenService = accessTokenService;


    //    }



    //    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    //    {
    //        try
    //        {
    //            var token = await _accessTokenService.GetToken();
    //            if (string.IsNullOrWhiteSpace(token)) 
    //            return await MarkAsUnauthorize();

    //            var readJwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
    //            var identity = new System.Security.Claims.ClaimsIdentity(readJwt.Claims, "JWT");
    //            var Principal = new System.Security.Claims.ClaimsPrincipal(identity);
    //            return await Task.FromResult(new AuthenticationState(Principal));
    //        }
    //        catch
    //        {
    //            return await MarkAsUnauthorize();

    //        }
    //    }

    //    private async Task<AuthenticationState> MarkAsUnauthorize()
    //    {
    //        try
    //        {
    //          var state = new AuthenticationState(new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity()));
    //            //await _accessTokenService.RemoveToken();
    //            NotifyAuthenticationStateChanged(Task.FromResult(state));
    //            return state;
    //        }
    //        catch(Exception ex) {
    //            Console.WriteLine($"Error marking as unauthorized: {ex.Message}");
    //            return new AuthenticationState(new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity()));

    //        }

    //    }
    //}
}
