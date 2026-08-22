using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Ai;
using ToMainApi.Models.Enums;
using ToMainApi.Services.AI;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NeuronNetworkController : ControllerBase
    {
        private readonly NeuronNetworkDispatcher _neuronNetworkDispatcher;
        private readonly IPromptService _promptService;
        private readonly IImageValidator _imageValidatorService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IApplicationPhotoService _applicationPhotoService;
        public NeuronNetworkController(NeuronNetworkDispatcher neuronNetworkDispatcher, 
            IPromptService promptService,
            IImageValidator imageValidationService, 
            ICloudinaryService cloudinaryService,
            IApplicationPhotoService applicationPhotoService)
        {
            _neuronNetworkDispatcher = neuronNetworkDispatcher;
            _promptService = promptService;
            _imageValidatorService = imageValidationService;
            _cloudinaryService = cloudinaryService;
            _applicationPhotoService = applicationPhotoService;
        }

        [Authorize(Roles ="Admin,Moderator")]
        [HttpGet("GetSupportedNeuronNetworks")]
        public IActionResult GetSupportedNeuronNetworks()
        {
            var providers = Enum.GetValues<SupportableAiProviders>()
                .Select(x => new AiProviderDto { Id = x, Name = x.ToString() }).ToList();
            return Ok(providers);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("GeneratePhoto")]
        public async Task <IActionResult> GeneratePhoto([FromBody] AiPhotoUploadRequest model)
        {
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var promptresult = await _promptService.GetPromptsByUserIdAndIds(userid, model.PromptsIds);
            var photodata = await _applicationPhotoService.GetApplicationPhotoByApplicationIdAndPhotoId(model.ApplicationId, model.PhotoId);
            if (promptresult.Success)
            {
                var photostream = await _cloudinaryService.DownloadPhotoAsStreamAsync(photodata.Data.Url);
                var strategy = _neuronNetworkDispatcher.GetStrategy(model.AiProvider);
                var photovalidateresult = _imageValidatorService.Validate(photostream);
                if (photovalidateresult.Success)
                {
                    var result = await strategy.ProcessPhotoAsync(photostream, promptresult.Data);
                    return Ok(result.Data);
                }
                return BadRequest(promptresult.Message);
            }
            return BadRequest();
        }
    }
}
