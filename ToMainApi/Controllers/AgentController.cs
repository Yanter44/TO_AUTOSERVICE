using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Agent;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _agentService;
        public AgentController(IAgentService agentService)
        {
            _agentService = agentService;
        }
       [Authorize(Roles = "Agent")]
       [HttpGet("GetMyData")]
       private async Task<IActionResult> GetMyData()
       {
            return Ok();
       }

       [Authorize(Roles = "Agent")]
       [HttpGet("CreateNewApplication")]
       private async Task<IActionResult> CreateNewApplication([FromBody] CreateNewApplicationDto model)
       {
          var result = await _agentService.CreateNewApplication(model);
          return Ok();
       }
    }
}
