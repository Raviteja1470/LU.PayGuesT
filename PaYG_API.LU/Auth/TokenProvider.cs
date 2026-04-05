using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PaYG_API.LU.Auth;
using PayingG.LU.API.Data.PayingGuest.Core.Models;
using System.Security.Claims;
using System.Text;

namespace PayingG.LU.API.Auth
{
    public class TokenProvider
    {
        private readonly IConfiguration _configuration;

        public TokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Token GenerateToken(UserAccount userAccount)
        {
            var accessToken = GenerateAccessToken(userAccount);
            var refreshToken = GenerateRefreshToken();

            return new Token
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }

        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Enabled = true
            };
        }
        private string GenerateAccessToken(UserAccount userAccount)
        {
            string secretKey = _configuration["JWT:SecretKey"]
                ?? throw new InvalidOperationException("JWT:Secret Key not found.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim("id", userAccount.Id.ToString()),
        new Claim("email", userAccount.Email),
        new Claim("role", userAccount.Role)
    };

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(30),
                SigningCredentials = credentials,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var handler = new JsonWebTokenHandler();
            return handler.CreateToken(descriptor);
        }
        //    private string GenerateAccessToken(UserAccount userAccount)
        //    {
        //        string secretKey = _configuration["Jwt:Key"]
        //            ?? throw new InvalidOperationException("Jwt:Key not found.");

        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        //        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        var claims = new List<Claim>
        //    {
        //        new Claim("id", userAccount.Id.ToString()),
        //        new Claim("email", userAccount.Email),
        //        new Claim("role", userAccount.Role)
        //    };

        //        var descriptor = new SecurityTokenDescriptor
        //        {
        //            Subject = new ClaimsIdentity(claims),
        //            Expires = DateTime.UtcNow.AddMinutes(30),
        //            SigningCredentials = credentials,
        //            Issuer = _configuration["Jwt:Issuer"],
        //            Audience = _configuration["Jwt:Audience"]
        //        };

        //        var handler = new JsonWebTokenHandler();
        //        return handler.CreateToken(descriptor);
        //    }
        //}

        //public class TokenProvider
        //{
        //    private readonly IConfiguration _configuration;

        //    public TokenProvider(IConfiguration configuration)
        //    {
        //        _configuration = configuration;
        //    }

        //    public Token GenerateToken(UserAccount userAccount)
        //    {
        //        var accessToken = GenerateAccessToken(userAccount);
        //        var refreshToken = GenerateRefreshToken();

        //        return new Token
        //        {
        //            AccessToken = accessToken,
        //            RefreshToken = refreshToken.Token

        //        };
        //    }

        //    private RefreshToken GenerateRefreshToken()
        //    {
        //        var refreshtoken = new RefreshToken
        //        {
        //            Token = Guid.NewGuid().ToString(),
        //            Expires = DateTime.UtcNow.AddDays(7),
        //            Enabled = true

        //        };
        //        return refreshtoken;

        //    }

        //    private string GenerateAccessToken(UserAccount userAccount)
        //    {
        //        string secretKey = _configuration["JWT:SecretKey"]
        //            ?? throw new InvalidOperationException("Secret key not found in configuration.");

        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        //        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        var claims = new List<Claim>
        //        {
        //            new Claim("id", userAccount.Id.ToString()),
        //            new Claim("email", userAccount.Email),
        //            new Claim("role", userAccount.Role)
        //        };

        //        var descriptor = new SecurityTokenDescriptor
        //        {
        //            Subject = new ClaimsIdentity(claims),
        //            Expires = DateTime.UtcNow.AddMinutes(30),
        //            SigningCredentials = credentials,
        //            Issuer = _configuration["JWT:Issuer"],
        //            Audience = _configuration["JWT:Audience"]

        //        };

        //        var handler = new JsonWebTokenHandler();
        //        return handler.CreateToken(descriptor);
        //    }
        //}

        public class Token
        {
            public string AccessToken { get; set; } = default!;
            public string RefreshToken { get; set; } = default!;
        }
    }
}