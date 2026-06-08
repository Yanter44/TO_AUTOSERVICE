namespace ToMainApi.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string plainText, string htmlContent);
    }
}
