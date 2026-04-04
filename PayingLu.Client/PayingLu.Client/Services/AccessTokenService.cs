namespace PayingLu.Client.Services
{
    public class AccessTokenService
    {
        private readonly CookieService _cookieService;
        private readonly string tokenKey = "access_token";

        public AccessTokenService(CookieService cookieService)
        {
            _cookieService = cookieService;
                        
        }



        public async Task SetToken(string token)
        {
            await _cookieService.SetCookie(tokenKey, token, 1);
        }

        public async  Task<string> GetToken()
        {
            return await _cookieService.GetCookie(tokenKey);
        }
         public async Task<bool> RemoveToken()
        {
            return await _cookieService.removeCookie(tokenKey);
        }
    }
}
