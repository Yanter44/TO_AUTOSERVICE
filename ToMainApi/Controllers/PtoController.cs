using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Prompt;
using ToMainApi.Models.Dtos.Pto;

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

        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("GetAllPtos")]
        public async Task<IActionResult> GetAllPtos()
        {
            var result = await _ptoservice.GetAllPtos();
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result.Message);
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
        public async Task<IActionResult> DeletePto([FromBody] DeletePtoRequestDto model)
        {
            var result = await _ptoservice.DeletePto(model);
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
