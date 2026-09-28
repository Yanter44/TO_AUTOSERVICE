using FluentValidation;
using ToMainApi.Models.Cloudinary;

namespace ToMainApi.Validators
{
    public class CloudinarySignatureRequestValidator : AbstractValidator<CloudinarySignatureRequest>
    {
        public CloudinarySignatureRequestValidator()
        {
            RuleFor(x => x.FileName).NotNull().NotEmpty();
            RuleFor(x => x.FileSize).NotNull().NotEmpty();
            RuleFor(x => x.FileType).NotNull().NotEmpty();
        }
    }
}
