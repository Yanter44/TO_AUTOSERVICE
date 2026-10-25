using Infisical.Sdk;
using Infisical.Sdk.Model;
using ToMainApi.Interfaces;

namespace ToMainApi.Services.Crypt
{
    public class InfisicalService : IInfisicalService
    {
        private readonly InfisicalClient _client;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _projectId;
        private readonly string _environmentSlug;

        public InfisicalService(IConfiguration configuration)
        {
            var settings = new InfisicalSdkSettingsBuilder()
                .WithHostUri("https://app.infisical.com") 
                .Build();
            _client = new InfisicalClient(settings);

            _projectId = configuration["Infisical:ProjectId"];
            _environmentSlug = configuration["Infisical:Environment"];
            _clientId = configuration["Infisical:ClientId"];
            _clientSecret = configuration["Infisical:ClientSecret"];
            
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            await _client.Auth().UniversalAuth().LoginAsync(_clientId, _clientSecret);
            var options = new ListSecretsOptions
            {
                EnvironmentSlug = _environmentSlug,
                ProjectId = _projectId,
                SecretPath = "/",
            };
            var secrets = await _client.Secrets().ListAsync(options);
            var secret = secrets?.FirstOrDefault(s => s.SecretKey == secretName);
            if (secret == null)
            {
                throw new Exception($"Секрет '{secretName}' не найден в Infisical.");
            }
            return secret.SecretValue;
        }
    }
}
