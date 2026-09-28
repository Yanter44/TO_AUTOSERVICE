using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.Refferal;
using ToMainApi.Models.Dtos.User;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _agentService;
        private readonly IApplicationService _applicationService;
        private readonly IRefferalService _refferalService;
        private ILogger<AgentController> _logger;
        public AgentController(IAgentService agentService, 
                               IApplicationService applicationService,
                               IRefferalService refferalService,
                               ILogger<AgentController> logger)
        {
            _agentService = agentService;
            _applicationService = applicationService;
            _refferalService = refferalService;
            _logger = logger;
        }

        [Authorize(Roles ="Agent")]
        [HttpGet("GetMyBalance")]
        public async Task<IActionResult> GetMyBalance()
        {
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _agentService.GetMyBalance(userid);
            return Ok(result);
        }

        [Authorize(Roles = "Agent")]
        [HttpGet("GetMyBranches")]
        public async Task<IActionResult> GetMyBranches()
        {
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userContextDto = new UserContextDto() { Id = userid };
            var result = await _refferalService.GetMyBranches(userContextDto);
            if(result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        
        [Authorize(Roles = "Agent")]
        [HttpPost("CreateBranch")]
        public async Task<IActionResult> CreateBranch([FromBody] CreateBranchDto model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userContextDto = new UserContextDto() { Id = userId};
            var result = await _refferalService.CreateBranch(userContextDto, model);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Agent")]
        [HttpGet("GetMyDebtLimit")]
        public async Task<IActionResult> GetMyDebtLimit()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _agentService.GetMyDebtLimit(userId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Agent")]
        [HttpGet("GetMyCurrentDebt")]
        public async Task<IActionResult> GetMyCurrentDebt()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _agentService.GetMyCurrentDebt(userId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Agent")]
        [HttpGet("GetMyAllBalanceTransactionStory")]
        public async Task <IActionResult> GetMyAllBalanceTransactionStory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _agentService.GetMyAllBalanceTransactionStory(userId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Agent")]
        [HttpGet("GetMyBalanceTransactionStory")]
        public async Task<IActionResult> GetMyBalanceTransactionStory([FromQuery] PaginationDto model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userContext = new UserContextDto() { Id = userId };
            var result = await _agentService.GetMyBalanceTransactionStory(userContext,model);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
