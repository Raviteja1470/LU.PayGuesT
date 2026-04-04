using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PaYG_API.LU.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ResourceController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [Route("verify")]
        public ActionResult verifyuser()
        {
            return Ok("user is authorized");
        }
    }
}
