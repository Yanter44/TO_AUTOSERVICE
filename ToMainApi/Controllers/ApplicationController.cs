using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Application;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        public ApplicationController(IApplicationService applicationservice)
        {
            _applicationService = applicationservice;
        }

        [Authorize]
        [HttpGet("GetAllApplications")]
        public async Task<IActionResult> GetAllApplications()
        {
            var result = await _applicationService.GetAllApplications();
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result.Message);
        }
        [Authorize]
        [HttpGet("DeleteApplication")]
        public async Task<IActionResult> DeleteApplication([FromBody] DeleteApplicationDto model)
        {
            var result = await _applicationService.DeleteApplication(model);
            if (result.Success)
                return Ok();
            return BadRequest(result.Message);
        }

    }
}
