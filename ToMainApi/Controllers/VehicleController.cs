using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Vehicle;
using ToMainApi.Models.Enums;

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
        [HttpGet("GetAllVehicleCategoriesBySrts")]
        public async Task<IActionResult> GetAllVehicleCategoriesBySrts()
        {
            var result = await _vehicleService.GetAllVehicleCategories(VehicleCategoryType.SRTS);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [Authorize]
        [HttpGet("GetAllVehicleCategoriesByOkp")]
        public async Task<IActionResult> GetAllVehicleCategoriesByOkp()
        {
            var result = await _vehicleService.GetAllVehicleCategories(VehicleCategoryType.OKP);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpPost("AddNewVehicleCategory")]
        public async Task<IActionResult> AddNewVehicleCategory([FromBody] AddNewVehicleCategoryDto model)
        {
            var result = await _vehicleService.AddNewVehicleCategory(model);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize]
        [HttpDelete("DeleteVehicleCategory")]
        public async Task<IActionResult> DeleteVehicleCategory([FromBody] DeleteVehicleCategoryDto model)
        {
            var result = await _vehicleService.DeleteVehicleCategory(model);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize]
        [HttpPut("UpdateVehicleCategory")]
        public async Task<IActionResult> UpdateVehicleCategory([FromBody] UpdateVehicleCategoryDto model)
        {
            var result = await _vehicleService.UpdateVehicleCategory(model);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
