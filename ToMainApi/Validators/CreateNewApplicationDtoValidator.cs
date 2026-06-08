using FluentValidation;
using ToMainApi.Models.Dtos.Agent;

namespace ToMainApi.Validators
{
    public class CreateNewApplicationDtoValidator
      : AbstractValidator<CreateNewApplicationDto>
    {
        public CreateNewApplicationDtoValidator()
        {

            RuleFor(x => x.VIN)
                .NotEmpty()
                .Length(17)
                .WithMessage("Вин должен иметь 17 знаков");

            RuleFor(x => x.GosNumber)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.Brand)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Model)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.YearOfRelease)
                .InclusiveBetween(1900, DateTime.UtcNow.Year + 1)
                .WithMessage("Неверная дата выхода");

            RuleFor(x => x.DocumentFiles)
                .NotNull()
                .Must(x => x.Count > 0)
                .WithMessage("Должно быть не менее одной фотографии");

            RuleFor(x => x.FIO)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(255);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.VehiclePhotos)
                .NotNull()
                .Must(x => x.Count > 0)
                .WithMessage("Должно быть не менее одной фотографии");

            RuleFor(x => x.PtoId)
                .GreaterThan(0)
                .WithMessage("Пто обязательно к заполнению");
        }
    }
}
