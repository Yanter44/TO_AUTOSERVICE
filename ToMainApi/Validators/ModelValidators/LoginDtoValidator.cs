using FluentValidation;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Dtos.Auth;

namespace ToMainApi.Validators.ModelValidators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().NotNull();
        }
    }
}
