using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayingG.LU.API.Auth;
using PayingG.LU.API.Data;
using PayingG.LU.API.Data.PayingGuest.Core.Models;

namespace PayingG.LU.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataAccess _dataAccess;
        private readonly AppDbContext _appDbContext;
        private readonly TokenProvider tokenProvider;

        public AuthController(DataAccess dataAccess,AppDbContext appDbContext, TokenProvider tokenProvider)
        {
            _dataAccess = dataAccess;
            _appDbContext = appDbContext;
            this.tokenProvider = tokenProvider;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserAccount request)
        {
            var result = await _dataAccess.RegisterUserAsync(
                request.Email,
                request.Password,
                request.Role
            );

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpPost("auth")]

        public async Task<ActionResult<AuthResponse>> Login(AuthRequest request)
        {
            AuthResponse authResponse = new AuthResponse();
            var user = await _appDbContext.UserAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return BadRequest("User is not found!");

            var verifyPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!verifyPassword)
                return BadRequest("Wrong password!");

            var token = tokenProvider.GenerateToken(user);
            authResponse.AccessToken = token.AccessToken;

            await _dataAccess.DisableUserTokenByEmailAsync(request.Email);

            authResponse.refreshToken = token.RefreshToken;
             await _dataAccess.AddRefreshTokenAsync(user.Email, token.RefreshToken, DateTime.UtcNow.AddDays(7));


            return Ok(authResponse);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Missing refresh token");

            var isValid = await _dataAccess.IsRefreshTokenValidAsync(refreshToken);
            if (!isValid)
                return BadRequest("Invalid refresh token");

            var currentUser = await _dataAccess.FindUserByTokenAsync(refreshToken);
            if (currentUser == null)
                return BadRequest("User not found");

            // Generate new tokens
            var token = tokenProvider.GenerateToken(currentUser);

            // Disable old refresh token
            await _dataAccess.DisableUserTokenAsync(refreshToken);

            // Insert new refresh token
            await _dataAccess.AddRefreshTokenAsync(
                currentUser.Email,
                token.RefreshToken,
                token.GetType().GetProperty("RefreshTokenExpiration")?.GetValue(token) as DateTime? ?? DateTime.UtcNow.AddDays(7)
            );

            var response = new AuthResponse
            {
                AccessToken = token.AccessToken,
                refreshToken = token.RefreshToken
            };

            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
                await _dataAccess.DisableUserTokenAsync(refreshToken);

            return Ok("Successfully LoggedOut");
        }

    }
}
