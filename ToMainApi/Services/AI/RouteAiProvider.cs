using System.Text;
using System.Text.Json;
using ToMainApi.Common;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Ai;
using ToMainApi.Models.Dtos.NeuronNetwork;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services.AI
{
    public class RouteAiProvider : INeuronNetwork
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        public RouteAiProvider(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _baseUrl = _configuration["RouteAiApi:BaseUrl"];
            _apiKey = _configuration["RouteAiApi:ApiKey"];
       

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.BaseAddress = new Uri(_baseUrl.TrimEnd('/') + "/");
        }

        public async Task<ServiceResponse<GeneratePhotoResponse>> ProcessPhotoAsync(Stream photo,  List<string> prompts, NeuronNetworkDto model)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                await photo.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                var base64Image = Convert.ToBase64String(imageBytes);
                var mimeType = "image/png"; 
                var prompt = string.Join(". ", prompts);
                var neuronNetworkModel = model.Link;

                var requestBody = new
                {
                    model = neuronNetworkModel,
                    prompt = prompt,
                    n = 1,
                    aspect_ratio = "16:9",
                    input_references = new[]
                    {
                        new
                        {
                            type = "image_url",
                            image_url = new { url = $"data:{mimeType};base64,{base64Image}" }
                        }
                    }
                };
                var endpoint = "images";
                var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpResponse = await _httpClient.PostAsync(endpoint, content);
                var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return new ServiceResponse<GeneratePhotoResponse>()
                    {
                        Success = false,
                        Message = $"API Ошибка ({httpResponse.StatusCode}): {jsonResponse}"
                    };
                }

                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                if (root.TryGetProperty("error", out var errorElement))
                {
                    return new ServiceResponse<GeneratePhotoResponse>()
                    {
                        Success = false,
                        Message = $"API error: {errorElement}"
                    };
                }

                if (!root.TryGetProperty("data", out var dataElement) ||
                    dataElement.ValueKind != JsonValueKind.Array ||
                    dataElement.GetArrayLength() == 0)
                {
                    return new ServiceResponse<GeneratePhotoResponse>()
                    {
                        Success = false,
                        Message = $"В ответе нет data: {jsonResponse}"
                    };
                }

                var first = dataElement[0];

                if (!first.TryGetProperty("b64_json", out var b64Element))
                {
                    return new ServiceResponse<GeneratePhotoResponse>()
                    {
                        Success = false,
                        Message = $"В data[0] нет b64_json: {jsonResponse}"
                    };
                }

                var base64Data = b64Element.GetString();
                if (string.IsNullOrEmpty(base64Data))
                {
                    return new ServiceResponse<GeneratePhotoResponse>()
                    {
                        Success = false,
                        Message = "b64_json пустой"
                    };
                }

                string mediaType = "image/png";
                if (first.TryGetProperty("media_type", out var mtElement))
                {
                    var mt = mtElement.GetString();
                    if (!string.IsNullOrEmpty(mt)) mediaType = mt;
                }

                return new ServiceResponse<GeneratePhotoResponse>()
                {
                    Data = new GeneratePhotoResponse()
                    {
                        ImageBase64 = base64Data,
                    },
                    Success = true,
                    Message = "Изображение успешно сгенерировано через RouterAI"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GeneratePhotoResponse>()
                {
                    Success = false,
                    Message = $"Произошла ошибка: {ex.Message}"
                };
            }
        }
    }
}
