using Microsoft.AspNetCore.Html;
using System.Net.Http.Headers;
using System.Text;
using ToMainApi.Common;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Ai;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services.AI
{
    public class ReveAiProvider : INeuronNetworkStrategy
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public SupportableAiProviders AiProvider => SupportableAiProviders.ReveAi;

        public ReveAiProvider(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            var apiKey = _configuration["ReveAi:ApiKey"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<ServiceResponse<GeneratePhotoResponse>> ProcessPhotoAsync(Stream photo, List<string> prompts)
        {
            using var ms = new MemoryStream();
            await photo.CopyToAsync(ms);

            var base64 = Convert.ToBase64String(ms.ToArray());
            var editInstruction = string.Join(Environment.NewLine, prompts);
            var payload = new
            {
                edit_instruction = $"{editInstruction}",
                reference_image = base64,
                aspect_ratio = "9:16",
                version = "latest"
            };
            var response = await _httpClient.PostAsJsonAsync("https://api.reve.com/v1/image/create", payload);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Email sending failed: {error}");
            }

            return new ServiceResponse<GeneratePhotoResponse>() { Success = true, };
        }
    }
}
