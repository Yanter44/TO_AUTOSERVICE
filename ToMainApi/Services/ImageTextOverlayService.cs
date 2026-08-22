
using SkiaSharp;
using ToMainApi.Interfaces;

namespace ToMainApi.Services
{
    public class ImageTextOverlayService : IimageTextOverlayService
    {

        public Task<string> AddText(Stream photo, string text)
        {
            using var original = SKBitmap.Decode(photo);

            using var surface = new SKBitmap(original.Width, original.Height);
            using var canvas = new SKCanvas(surface);

            // рисуем фото
            canvas.DrawBitmap(original, 0, 0);

            using var font = new SKFont
            {
                Size = 36
            };

            using var paint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true
            };

            float x = 20;
            float y = surface.Height - 40;

            canvas.DrawText(text, x, y, font, paint);

            using var image = SKImage.FromBitmap(surface);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);

            var bytes = data.ToArray();

            return Task.FromResult(Convert.ToBase64String(bytes));
        }
    }
}
