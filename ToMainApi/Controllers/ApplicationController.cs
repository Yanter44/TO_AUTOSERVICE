using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : ControllerBase
    {
        public ApplicationController()
        {
            
        }

        [Authorize]
        [HttpGet("GetAllApplications")]
        public async Task<IActionResult> GetAllApplications()
        {
            return Ok();
        }

    }
}
