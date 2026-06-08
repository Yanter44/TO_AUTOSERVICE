namespace ToMainApi.Models.Dtos.Auth
{
    public class ConfirmCodeDto
    {
        public string RegistrationId { get; set; }
        public string Code { get; set; }
    }
}
