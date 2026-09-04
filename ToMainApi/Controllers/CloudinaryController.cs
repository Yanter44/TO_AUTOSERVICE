using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Cloudinary;
using ToMainApi.Models.Entities;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CloudinaryController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<CloudinaryController> _logger;
        public CloudinaryController(ICloudinaryService cloudinaryService,ILogger<CloudinaryController> logger)
        {
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }
        [Authorize(Roles = "Agent")]
        [HttpPost("GetUploadSignature")]
        public async Task<IActionResult> GetUploadSignature([FromBody] CloudinarySignatureRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _cloudinaryService.GenerateSignatureAsync(userId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при генерации подписи");
                return StatusCode(500);
            }
        }
    }
}
