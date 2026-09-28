using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.PhotoGallery;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PhotoGalleryController : ControllerBase
    {
        private readonly IPhotoGalleryService _photoGalleryService;
        public PhotoGalleryController(IPhotoGalleryService photoGalleryService)
        {
            _photoGalleryService = photoGalleryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetPhotoGallery")]
        public async Task<IActionResult> GetPhotoGallery([FromQuery] PaginationDto model)
        {
            var result = await _photoGalleryService.GetPhotoGallery(model);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddNewPhotoToGallery")]
        public async Task<IActionResult> AddNewPhotoToGallery([FromBody] AddNewPhotoToGalleryDto model)
        {
            var result = await _photoGalleryService.AddNewPhotoToGallery(model);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeletePhotoFromGallery")]
        public async Task<IActionResult> DeletePhotoFromGallery([FromQuery] int photoId)
        {
            var result = await _photoGalleryService.DeletePhotoFromPhotoGallery(photoId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
