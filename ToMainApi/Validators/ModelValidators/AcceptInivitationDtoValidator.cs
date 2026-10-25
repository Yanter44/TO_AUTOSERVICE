using FluentValidation;
using ToMainApi.Models.Dtos.Invitation;

namespace ToMainApi.Validators.ModelValidators
{
    public class AcceptInivitationDtoValidator : AbstractValidator<AcceptInvitationDto>
    {
        public AcceptInivitationDtoValidator()
        {
            RuleFor(x => x.Token).NotNull().NotEmpty();
            RuleFor(x => x.FIO).NotNull().NotEmpty();
            RuleFor(x => x.Password).NotNull().NotEmpty();
        }
    }
}
