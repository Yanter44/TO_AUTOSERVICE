using System.Net.Http.Headers;
using ToMainApi.Common;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Ai;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services.AI
{
    public class NanoBananaAiProvider : INeuronNetworkStrategy
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public SupportableAiProviders AiProvider => SupportableAiProviders.NanoBanana;

        public NanoBananaAiProvider(HttpClient httpclient, IConfiguration configuration)
        {
            _httpClient = httpclient;
            _configuration = configuration;
            var apiKey = _configuration["NanoBanana:ApiKey"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<ServiceResponse<GeneratePhotoResponse>> ProcessPhotoAsync(Stream photo, List<string> prompts)
        {
            // вызов API NanoBanana

            return new ServiceResponse<GeneratePhotoResponse>() { 
            };
        }
    }
}
