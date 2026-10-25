using SkiaSharp;
using ToMainApi.Common;
using ToMainApi.Interfaces;

public class ImageTextOverlayService : IimageTextOverlayService
{
    private const float Padding = 20f;
    private const float FontSize = 36f;
    private const float LineSpacing = 1.2f;  
    private const int JpegQuality = 90;

    public async Task<ServiceResponse<MemoryStream>> AddText(Stream photo, IReadOnlyList<string> lines)
    {
        if (photo is null || !photo.CanRead)
            return new ServiceResponse<MemoryStream> { Success = false, Message = "Файл не передан" };

        if (lines is null || lines.Count == 0)
            return new ServiceResponse<MemoryStream> { Success = false, Message = "Нет текста для наложения" };

        if (photo.CanSeek && photo.Position != 0)
            photo.Position = 0;

        try
        {
            using var original = SKBitmap.Decode(photo);
            if (original is null)
                return new ServiceResponse<MemoryStream> { Success = false, Message = "Не удалось декодировать изображение" };

            using var surface = new SKBitmap(original.Width, original.Height);
            using var canvas = new SKCanvas(surface);
            canvas.DrawBitmap(original, 0, 0);

            using var font = new SKFont { Size = FontSize };
            using var paint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true,
            };
            var lineHeight = FontSize * LineSpacing;
            var totalTextHeight = lines.Count * lineHeight;

            var bottomY = surface.Height - Padding;
            var firstLineY = bottomY - (lines.Count - 1) * lineHeight;

            for (var i = 0; i < lines.Count; i++)
            {
                var y = firstLineY + i * lineHeight;
                canvas.DrawText(lines[i], Padding, y, font, paint);
            }

            using var image = SKImage.FromBitmap(surface);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, JpegQuality);

            var ms = new MemoryStream();  
            try
            {
                data.SaveTo(ms);
                ms.Position = 0;
                return new ServiceResponse<MemoryStream> { Data = ms, Success = true };
            }
            catch
            {
                await ms.DisposeAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            return new ServiceResponse<MemoryStream> { Success = false, Message = "Ошибка при наложении текста" };
        }
    }
}