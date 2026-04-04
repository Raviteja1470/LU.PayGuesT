using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;

namespace PayingLu.Client.Security
{
    public class JWTAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public JWTAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var token = Request.Cookies["access_token"];
                if (string.IsNullOrWhiteSpace(token))
                    return await Task.FromResult(AuthenticateResult.Fail("No token found"));

                var readJwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var identity = new System.Security.Claims.ClaimsIdentity(readJwt.Claims, "JWT");
                var Principal = new System.Security.Claims.ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(Principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch
            {
                return await Task.FromResult(AuthenticateResult.Fail("No token found"));

            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Redirect("/login");
            await Task.CompletedTask;
        }

        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.Redirect("/AccessDenined");
            await Task.CompletedTask;
        }

        // Ensure this local options type inherits from the framework's AuthenticationSchemeOptions
        public class AuthenticationSchemeOptions : Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions
        {

        }



    }
}
