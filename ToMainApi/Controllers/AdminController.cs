using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToMainApi.Common;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Admin;
using ToMainApi.Models.Dtos.Pagination;
using static System.Net.Mime.MediaTypeNames;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IimageMetadataEditor _metadataEditor;
        private readonly IimageTextOverlayService _imagetextoverlayservice;
        public AdminController(IimageMetadataEditor metadataEditor, IimageTextOverlayService imagetextoverlayservice)
        {
            _metadataEditor = metadataEditor;
            _imagetextoverlayservice = imagetextoverlayservice;
        }

        [HttpPost("metadataphoto")]
        public async Task<IActionResult> metadataphoto(IFormFile photo)
        {
            using var stream = photo.OpenReadStream();
            var result = await _metadataEditor.ProcessImage(stream);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("AddTextToPhoto")]
        public async Task<IActionResult> AddTextToPhoto(IFormFile photo, [FromQuery] List<string> textToWrite)
        {
            using var stream = photo.OpenReadStream();
            var result = await _imagetextoverlayservice.AddText(stream, textToWrite);
            await using var ms = result.Data;
            using var buffer = new MemoryStream();
            await ms.CopyToAsync(buffer);

            return Ok( Convert.ToBase64String(buffer.ToArray()));
        }
    }
}
