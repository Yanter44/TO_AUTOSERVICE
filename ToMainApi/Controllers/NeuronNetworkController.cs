using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Ai;
using ToMainApi.Models.Dtos.NeuronNetwork;
using ToMainApi.Models.Enums;
using ToMainApi.Services.AI;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Common;
using ToMainApi.Models.Dtos.ApplicationPhoto;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NeuronNetworkController : ControllerBase
    {
        private readonly INeuronNetworkService _neuronNetworkService;
        private readonly INeuronNetwork _neuronNetwork;
        private readonly IPromptService _promptService;
        private readonly IImageValidator _imageValidatorService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IApplicationPhotoService _applicationPhotoService;
        public NeuronNetworkController(INeuronNetworkService neuronNetworkService,
            INeuronNetwork neuronNetwork,
            IPromptService promptService,
            IImageValidator imageValidationService, 
            ICloudinaryService cloudinaryService,
            IApplicationPhotoService applicationPhotoService)
        {
            _neuronNetworkService = neuronNetworkService;
            _neuronNetwork = neuronNetwork;
            _promptService = promptService;
            _imageValidatorService = imageValidationService;
            _cloudinaryService = cloudinaryService;
            _applicationPhotoService = applicationPhotoService;
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("GetAllNeuronNetworks")]
        public async Task<IActionResult> GetAllNeuronNetworks()
        {
            var result = await _neuronNetworkService.GetAllNeuronNetworks();
            if(result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("GetNeuronNetworks")]
        public async Task<IActionResult> GetNeuronNetworks([FromQuery] PaginationDto model)
        {
            var result = await _neuronNetworkService.GetNeuronNetworks(model);
            if(result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("AddNewNeuronNetwork")]
        public async Task<IActionResult> AddNewNeuronNetwork([FromBody] AddNewNeuronNetworkDto model)
        {
            var result = await _neuronNetworkService.AddNewNeuronNetwork(model);
            if(result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpDelete("DeleteNeuronNetwork")]
        public async Task<IActionResult> DeleteNeuronNetwork([FromQuery] int neuronNetworkId)
        {
            var result = await _neuronNetworkService.DeleteNeuronNetwork(neuronNetworkId);
            if(result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPut("UpdateNeuronNetwork")]
        public async Task<IActionResult> UpdateNeuronNetwork([FromBody] UpdateNeuronNetworkDto model)
        {
            var result = await _neuronNetworkService.UpdateNeuronNetwork(model);
            if(result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("GeneratePhoto")]
        public async Task <IActionResult> GeneratePhoto([FromBody] AiPhotoUploadRequest model)
        {
            var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var promptresult = await _promptService.GetPromptsByUserIdAndIds(userid, model.PromptsIds);
            var neuronNetworkResult = await _neuronNetworkService.GetNeuronNetworkById(model.NeuronNetworkId);
            var photodata = await _applicationPhotoService.GetApplicationPhotoByApplicationIdAndPhotoId(model.ApplicationId, model.PhotoId);
            if (promptresult.Success && neuronNetworkResult.Success)
            {
                var photostreamresult = await _cloudinaryService.DownloadPhotoAsStreamAsync(photodata.Data.Url);
                var photovalidateresult = _imageValidatorService.Validate(photostreamresult.Data);
                if (photovalidateresult.Success && photostreamresult.Success)
                {
                    var result = await _neuronNetwork.ProcessPhotoAsync(photostreamresult.Data, promptresult.Data, neuronNetworkResult.Data);
                    return Ok(result);
                }
                return BadRequest(promptresult);
            }
            return BadRequest();
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("ConfirmGeneratedPhoto")]
        public async Task<IActionResult> ConfirmGeneratedPhoto([FromBody] ConfirmAIGeneratedPhotoDto model)
        {
            var result = await _applicationPhotoService.ConfirmGeneratedPhoto(model);
            if (result.Success)
                return Ok();
            return BadRequest();
        }
    }
}
