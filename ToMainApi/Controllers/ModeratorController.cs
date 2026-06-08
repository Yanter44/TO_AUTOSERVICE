using Microsoft.AspNetCore.Mvc;
using ToMainApi.Interfaces;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModeratorController : ControllerBase
    {
        private IModeratorService _moderatorService;
        public ModeratorController(IModeratorService moderatorService)
        {
            _moderatorService = moderatorService;
        }
    }
}
