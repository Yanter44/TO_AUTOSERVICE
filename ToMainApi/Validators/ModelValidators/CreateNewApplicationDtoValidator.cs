using FluentValidation;
using ToMainApi.Models.Dtos.Agent;

namespace ToMainApi.Validators.ModelValidators
{
    public class CreateNewApplicationDtoValidator : AbstractValidator<CreateNewApplicationDto>
    {
        public CreateNewApplicationDtoValidator()
        {
            RuleFor(x => x.VIN)
                 .NotEmpty().WithMessage("VIN обязателен для заполнения")
                 .Length(17)
                 .WithMessage("VIN: 17 знаков, только латиница (кроме I, O, Q) и цифры")
                 .When(x => !x.IsVinMissing);

            RuleFor(x => x.VehicleYearOfRelease)
               .InclusiveBetween(1850, DateTime.UtcNow.Year)
               .WithMessage($"Год выпуска ТС должен быть в диапазоне 1850–{DateTime.UtcNow.Year}");

            RuleFor(x => x.Brand)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Model)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.GosNumber)
                .NotEmpty()
                .MaximumLength(20).WithMessage("Максимальная длина - 20 символов");

            RuleFor(x => x.VehicleBodyNumber)
              .MaximumLength(40).WithMessage("Кузов № — не более 40 символов")
              .WithMessage("Кузов №: минимум одна латинская/кириллическая буква или цифра")
              .When(x => !string.IsNullOrWhiteSpace(x.VehicleBodyNumber));

            RuleFor(x => x.ChassisNumber)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(40).WithMessage("Шасси № — не более 40 символов")
                .WithMessage("Шасси №: только буквы, дефис и пробел")
                .When(x => !x.IsChassisNumberMissing);

            RuleFor(x => x.AdditionalMarking)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Доп. маркировка должна быть неотрицательной")
                .When(x => x.AdditionalMarking.HasValue);

            RuleFor(x => x.VehicleMileage)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Пробег ТС не может быть отрицательным");

            RuleFor(x => x.GosNumber)
                .NotEmpty().WithMessage("Гос. номер обязателен")
                .MaximumLength(10).WithMessage("Гос. номер — не более 10 символов");

            RuleFor(x => x.MassWithoutLoad)
                .GreaterThan(0).WithMessage("Масса без нагрузки обязательна");

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
