namespace ToMainApi.Models.Dtos.Refferal
{
    public class BranchMemberDto
    {
        public int AgentId { get; set; }
        public string Fio { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public decimal DebitLimit { get; set; }
        public decimal CurrentDebit { get; set; }

        public DateTime RegDate { get; set; }
    }
}
