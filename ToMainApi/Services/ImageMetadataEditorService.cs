using ImageMagick;
using ToMainApi.Common;
using ToMainApi.Interfaces;

namespace ToMainApi.Services
{
    public class ImageMetadataEditorService : IimageMetadataEditor
    {
        public async Task<ServiceResponse<string>> ProcessImage(Stream photo)
        {
            if (photo == null || !photo.CanRead)
            {
                return new ServiceResponse<string>() { Success = false };
            }

            try
            {
                using var image = new MagickImage(photo);

                var originalAuthor = image.GetAttribute("Artist");

                image.SetAttribute("Artist", "My Metadata Service");
                image.SetAttribute("Comment", "Processed by API");
                image.SetAttribute("Software", "ImageMetadataEditorService");

                using var ms = new MemoryStream();
                await image.WriteAsync(ms);
                ms.Position = 0;

                var base64 = Convert.ToBase64String(ms.ToArray());

                return new ServiceResponse<string>() { Data = base64, Success = true};
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>() { Success = false };
            }
        }
    }
}
