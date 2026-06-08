using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ToMainApi.Interfaces;

namespace ToMainApi.Services
{
    public class MailerSendService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MailerSendService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            var apiKey = _configuration["MailerSend:ApiKey"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string plainText, string htmlContent)
        {
            var payload = new
            {
                from = new { email = _configuration["MailerSend:FromEmail"], name = "TO-AutoService" },
                to = new[] { new { email = toEmail } },
                subject = subject,
                text = plainText,
                html = htmlContent
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.mailersend.com/v1/email", payload);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Email sending failed: {error}");
            }
        }
    }
}
