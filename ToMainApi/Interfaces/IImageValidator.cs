using ToMainApi.Common;

namespace ToMainApi.Interfaces
{
    public interface IImageValidator
    {
        ServiceResponse<bool> Validate(Stream image);
    }
}
