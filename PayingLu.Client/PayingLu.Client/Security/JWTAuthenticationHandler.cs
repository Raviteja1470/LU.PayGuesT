using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var token = Request.Cookies["access_token"];
                if (string.IsNullOrWhiteSpace(token))
                    return Task.FromResult(AuthenticateResult.Fail("No token found"));

                var readJwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var claims = readJwt.Claims.ToList();

                // add raw token as claim for UI
                claims.Add(new Claim("access_token", token));

                var identity = new ClaimsIdentity(claims, "JWT");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
            }
        }
        //protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        //{
        //    try
        //    {
        //        var token = Request.Cookies["access_token"];
        //        if (string.IsNullOrWhiteSpace(token))
        //            return AuthenticateResult.Fail("No token found");

        //        var readJwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        //        var identity = new ClaimsIdentity(readJwt.Claims, "JWT");
        //        var principal = new ClaimsPrincipal(identity);
        //        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        //        return AuthenticateResult.Success(ticket);
        //    }
        //    catch
        //    {
        //        return AuthenticateResult.Fail("Invalid token");
        //    }
        //}

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
