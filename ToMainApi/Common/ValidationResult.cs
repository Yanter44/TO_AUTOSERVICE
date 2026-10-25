namespace ToMainApi.Common
{
    public class ValidationResult
    {
        public bool IsValid { get; init; }
        public List<string> Errors { get; init; } = new();

        public static ValidationResult Ok() => new() { IsValid = true };
        public static ValidationResult Fail(params string[] errors) =>
            new() { IsValid = false, Errors = errors.ToList() };
    }
}
