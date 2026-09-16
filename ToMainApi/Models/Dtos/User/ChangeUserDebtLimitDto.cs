namespace ToMainApi.Models.Dtos.User
{
    public class ChangeUserDebtLimitDto
    {
        public int UserId { get; set; }
        public decimal DebtLimit { get; set; }
    }
}
