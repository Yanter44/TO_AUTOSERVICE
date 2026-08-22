using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Vehicle;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }
        [Authorize]
        [HttpGet("GetAllVehicleCategories")]
        public async Task<IActionResult> GetAllVehicleCategories()
        {
           var result = await _vehicleService.GetAllVehicleCategories();
           if (result.Success)
               return Ok(result.Data);

           return BadRequest();
        }
        [Authorize]
        [HttpPost("AddNewVehicleCategory")]
        public async Task<IActionResult> AddNewVehicleCategory([FromBody] AddNewVehicleCategoryDto model)
        {
            var result = await _vehicleService.AddNewVehicleCategory(model);
            if (result.Success)
                return Ok();
            return BadRequest(result.Message);
        }
        [Authorize]
        [HttpDelete("DeleteVehicleCategory")]
        public async Task<IActionResult> DeleteVehicleCategory([FromBody] DeleteVehicleCategoryDto model)
        {
            var result = await _vehicleService.DeleteVehicleCategory(model);
            if (result.Success)
                return Ok();
            return BadRequest(result.Message);
        }
        [Authorize]
        [HttpPut("UpdateVehicleCategory")]
        public async Task<IActionResult> UpdateVehicleCategory([FromBody] UpdateVehicleCategoryDto model)
        {
            var result = await _vehicleService.UpdateVehicleCategory(model);
            if (result.Success)
                return Ok();
            return BadRequest(result.Message);
        }
    }
}
