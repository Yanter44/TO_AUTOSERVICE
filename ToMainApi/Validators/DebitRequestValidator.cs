using FluentValidation;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Payments;

namespace ToMainApi.Validators
{
    public class DebitRequestValidator : AbstractValidator<DebitRequest>
    {
        public DebitRequestValidator()
        {
            RuleFor(x => x.IdempotencyKey).NotEmpty().NotNull();
            RuleFor(x => x.Amount).GreaterThan(1);
            RuleFor(x => x.AgentId).NotNull();
        }
    }
}
