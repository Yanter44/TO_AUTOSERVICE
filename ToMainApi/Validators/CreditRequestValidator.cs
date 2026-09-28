using CloudinaryDotNet.Actions;
using FluentValidation;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Payments;

namespace ToMainApi.Validators
{
    public class CreditRequestValidator : AbstractValidator<CreditRequest>
    {
        public CreditRequestValidator()
        {
            RuleFor(x => x.IdempotencyKey).NotNull().NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.AgentId).NotNull();
        }
    }
}
