namespace ToMainApi.Models.Dtos.Admin
{
    public class ChangeUserDebtLimitDto
    {
        public int UserId { get; set; }
        public decimal NewDebtLimit { get; set; }
    }
}
