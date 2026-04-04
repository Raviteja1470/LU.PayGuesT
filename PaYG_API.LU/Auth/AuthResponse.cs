namespace PayingG.LU.API.Auth
{
    public class AuthResponse
    {
        public string ?AccessToken { get; set; }
        public string? refreshToken { get; set; }
    }
}
