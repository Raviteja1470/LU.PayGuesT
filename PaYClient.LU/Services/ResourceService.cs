namespace PaYClient.LU.Services
{
    public class ResourceService
    {
        private readonly APIService aPIService;
        public ResourceService(APIService aPIService)
        {
            this.aPIService = aPIService ?? throw new ArgumentNullException(nameof(aPIService));
        }

        public async Task<bool> Verify()
        {
            var response = await aPIService.GetAsync("Resource/verify");
            return response.IsSuccessStatusCode;
        }
    }
}
