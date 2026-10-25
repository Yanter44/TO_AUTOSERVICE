using ToMainApi.Common;

namespace ToMainApi.Validators.CommonValidators
{
    public static class FileUploadValidator
    {
        private static readonly string[] BlockedMimeTypes =
        {
            "application/x-msdownload",
            "application/x-executable",
            "text/html",
            "application/javascript",
            "application/x-php",
            "application/x-msdos-program",
            "application/java-archive"
        };

        public const long MaxPhotoSize = 10 * 1024 * 1024;

        public static ValidationResult ValidatePhoto(string contentType, long size)
        {
            var errors = new List<string>();

            if (BlockedMimeTypes.Any(b => contentType.Contains(b, StringComparison.OrdinalIgnoreCase)))
                errors.Add("Загрузка файлов этого типа запрещена");

            if (size > MaxPhotoSize)
                errors.Add($"Фото слишком большое. Максимум {MaxPhotoSize / 1024 / 1024} MB");

            return errors.Count == 0
                ? ValidationResult.Ok()
                : ValidationResult.Fail(errors.ToArray());
        }

        public static string GetPhotoFolder(int userId) =>
            $"applications/{userId}/photos/{DateTime.UtcNow:yyyy-MM-dd}";
    }
}
