using ImageMagick;
using ToMainApi.Common;
using ToMainApi.Interfaces;

namespace ToMainApi.Services
{
    public class ImageMetadataEditorService : IimageMetadataEditor
    {
        private readonly ILogger<ImageMetadataEditorService> _logger;

        public ImageMetadataEditorService(ILogger<ImageMetadataEditorService> logger)
        {
            _logger = logger;
        }
        public async Task<ServiceResponse<MemoryStream>> ProcessImage(Stream photo)
        {
            if (photo is null || !photo.CanRead)
                return new ServiceResponse<MemoryStream> { Success = false, Message = "Файл не передан" };

            try
            {
                using var image = new MagickImage(photo);

                image.SetAttribute("Artist", "My Metadata Service");
                image.SetAttribute("Comment", "Processed by API");
                image.SetAttribute("Software", "ImageMetadataEditorService");

                var ms = new MemoryStream();
                try
                {
                    await image.WriteAsync(ms);
                    ms.Position = 0;

                    return new ServiceResponse<MemoryStream>
                    {
                        Data = ms,
                        Success = true,
                    };
                }
                catch
                {
                    await ms.DisposeAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обработать метаданные фото");
                return new ServiceResponse<MemoryStream> { Success = false, Message = ResponseMessages.Error };
            }
        }
    }
}
