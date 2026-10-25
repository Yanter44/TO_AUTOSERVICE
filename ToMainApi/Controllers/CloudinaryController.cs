using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToMainApi.Common;
using ToMainApi.Interfaces;
using ToMainApi.Models.Cloudinary;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Enums;
using ToMainApi.Validators.CommonValidators;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CloudinaryController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<CloudinaryController> _logger;

        private static readonly Dictionary<UploadPhotoScenario, string[]> ScenarioRoles = new()
        {
            [UploadPhotoScenario.AgentUploadApplicationPhoto] = new[] { "Agent", "Admin" },
            [UploadPhotoScenario.AdminUploadLibraryPhoto] = new[] { "Admin" }
        };

        public CloudinaryController(
            ICloudinaryService cloudinaryService,
            ILogger<CloudinaryController> logger)
        {
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("GetUploadPhotoSignature")]
        public async Task<IActionResult> GetUploadPhotoSignature([FromBody] CloudinarySignatureRequest request)
        {
            try
            {
                if (!Enum.TryParse<UploadPhotoScenario>(request.Scenario, ignoreCase: true, out var scenario)
                    || !Enum.IsDefined(scenario))
                {
                    return BadRequest(new ServiceResponse<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "Неизвестный сценарий загрузки"
                    });
                }

                if (!CanUseScenario(scenario))
                    return Forbid();

                var validation = FileUploadValidator.ValidatePhoto(request.FileType, request.FileSize);
                if (!validation.IsValid)
                {
                    return BadRequest(new ServiceResponse<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = string.Join("; ", validation.Errors)
                    });
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userContext = new UserContextDto { Id = userId };

                var result = await _cloudinaryService.GenerateSignatureAsync(userContext, scenario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при генерации подписи");
                return StatusCode(500);
            }
        }

        private bool CanUseScenario(UploadPhotoScenario scenario)
        {
            if (!ScenarioRoles.TryGetValue(scenario, out var allowedRoles))
                return false;

            return allowedRoles.Any(User.IsInRole);
        }
    }
}