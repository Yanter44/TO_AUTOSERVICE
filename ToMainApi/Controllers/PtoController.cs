using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.Prompt;
using ToMainApi.Models.Dtos.Pto;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Services;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PtoController : ControllerBase
    {
        private readonly IPtoService _ptoservice;
        public PtoController(IPtoService ptoservice)
        {
            _ptoservice = ptoservice;
        }

        [Authorize]
        [HttpGet("GetAllPtos")]
        public async Task<IActionResult> GetAllPtos()
        {
            var result = await _ptoservice.GetAllPtos();
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result.Message);
        }

        [Authorize]
        [HttpGet("GetPtos")] 
        public async Task<IActionResult> GetPtos([FromQuery] PaginationDto model)
        {
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var usercontext = new UserContextDto() { Id = userid };
            var result = await _ptoservice.GetPtos(usercontext, model);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize(Roles ="Admin,Moderator")]
        [HttpPost("AddNewPto")]
        public async Task<IActionResult> AddNewPto([FromBody] AddNewPtoDto model)
        {
            var result = await _ptoservice.AddNewPto(model);
            if (result.Success)
                return Ok();
            return BadRequest(result.Message);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpDelete("DeletePto")]
        public async Task<IActionResult> DeletePto([FromQuery] int ptoId)
        {
            var result = await _ptoservice.DeletePto(ptoId);
            if (result.Success)
                return Ok();
            return BadRequest(result.Message);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPut("UpdatePto")]
        public async Task<IActionResult> UpdatePto([FromBody] UpdatePtoRequestDto model)
        {
            var result = await _ptoservice.UpdatePto(model);
            if (result.Success)
                return Ok();
            return BadRequest(result.Message);
        }
    }
}
