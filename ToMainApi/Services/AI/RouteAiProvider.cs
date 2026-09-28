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

        public async Task<ServiceResponse<GeneratePhotoResponse>> ProcessPhotoAsync(IReadOnlyList<Stream> photos, List<string> prompts, NeuronNetworkDto model)
        {
            try
            {
                if (photos == null || photos.Count == 0)
                {
                    return new ServiceResponse<GeneratePhotoResponse>
                    {
                        Success = false,
                        Message = "Нет фото для генерации"
                    };
                }

                var inputReferences = new List<object>(photos.Count);

                foreach (var photo in photos)
                {
                    if (photo.CanSeek) photo.Position = 0;

                    using var memoryStream = new MemoryStream();
                    await photo.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();

                    var mimeType = DetectMimeType(imageBytes);  
                    var base64Image = Convert.ToBase64String(imageBytes);

                    inputReferences.Add(new
                    {
                        type = "image_url",
                        image_url = new { url = $"data:{mimeType};base64,{base64Image}" }
                    });
                }

                var prompt = string.Join(". ", prompts);
                var neuronNetworkModel = model.Link;

                var requestBody = new
                {
                    model = neuronNetworkModel,
                    prompt = prompt,
                    n = 1,
                    aspect_ratio = "16:9",
                    input_references = inputReferences  
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
                    return new ServiceResponse<GeneratePhotoResponse>
                    {
                        Success = false,
                        Message = $"API Ошибка ({httpResponse.StatusCode}): {jsonResponse}"
                    };
                }

                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                if (root.TryGetProperty("error", out var errorElement))
                {
                    return new ServiceResponse<GeneratePhotoResponse>
                    {
                        Success = false,
                        Message = $"API error: {errorElement}"
                    };
                }

                if (!root.TryGetProperty("data", out var dataElement) ||
                    dataElement.ValueKind != JsonValueKind.Array ||
                    dataElement.GetArrayLength() == 0)
                {
                    return new ServiceResponse<GeneratePhotoResponse>
                    {
                        Success = false,
                        Message = $"В ответе нет data: {jsonResponse}"
                    };
                }

                var first = dataElement[0];

                if (!first.TryGetProperty("b64_json", out var b64Element))
                {
                    return new ServiceResponse<GeneratePhotoResponse>
                    {
                        Success = false,
                        Message = $"В data[0] нет b64_json: {jsonResponse}"
                    };
                }

                var base64Data = b64Element.GetString();
                if (string.IsNullOrEmpty(base64Data))
                {
                    return new ServiceResponse<GeneratePhotoResponse>
                    {
                        Success = false,
                        Message = "b64_json пустой"
                    };
                }

                return new ServiceResponse<GeneratePhotoResponse>
                {
                    Data = new GeneratePhotoResponse { ImageBase64 = base64Data },
                    Success = true,
                    Message = "Изображение успешно сгенерировано через RouterAI"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GeneratePhotoResponse>
                {
                    Success = false,
                    Message = $"Произошла ошибка: {ex.Message}"
                };
            }
        }

        private static string DetectMimeType(byte[] bytes)
        {
            if (bytes.Length >= 8 &&
                bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                return "image/png";

            if (bytes.Length >= 3 &&
                bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                return "image/jpeg";

            if (bytes.Length >= 12 &&
                bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 &&
                bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50)
                return "image/webp";

            // на всякий случай
            return "image/png";
        }
    }
}
