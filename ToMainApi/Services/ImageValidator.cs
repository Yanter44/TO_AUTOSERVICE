using ToMainApi.Common;
using ToMainApi.Interfaces;

namespace ToMainApi.Services
{
    public class ImageValidator : IImageValidator
    {
        private const long MaxSize = 10 * 1024 * 1024;

        public ServiceResponse<bool> Validate(Stream image)
        {
            if (image == null || image.Length == 0)
                return new ServiceResponse<bool> { Success = false, Message = "Файл пуст" };

            if (image.Length > MaxSize)
                return new ServiceResponse<bool> { Success = false, Message = "Файл слишком большой" };

            image.Position = 0;
            Span<byte> header = stackalloc byte[8];
            image.ReadExactly(header);
            image.Position = 0;

            // PNG
            if (header[0] == 0x89 && header[1] == 0x50)
                return new ServiceResponse<bool>() { Success = true };

            // JPG
            if (header[0] == 0xFF && header[1] == 0xD8)
                return new ServiceResponse<bool>() { Success = true };

            return new ServiceResponse<bool>() { Success = false, Message = "Неверный формат файла" };
        }
    }
}
