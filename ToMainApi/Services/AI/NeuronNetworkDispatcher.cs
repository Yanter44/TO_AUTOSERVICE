using ToMainApi.Interfaces;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services.AI
{
    public class NeuronNetworkDispatcher
    {
        private readonly IReadOnlyDictionary<SupportableAiProviders, INeuronNetworkStrategy> _strategies;

        public NeuronNetworkDispatcher(IEnumerable<INeuronNetworkStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(x => x.AiProvider);
        }

        public INeuronNetworkStrategy GetStrategy(SupportableAiProviders provider)
        {
            if (_strategies.TryGetValue(provider, out var strategy))
            {
                return strategy;
            }

            throw new NotSupportedException($"Провайдер {provider} не поддерживается.");
        }
    }
}
