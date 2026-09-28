namespace ToMainApi.Models.Dtos.Refferal
{
    public class BranchWithMembersDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Fee { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MemberCount { get; set; }
        public List<BranchMemberDto> Members { get; set; } = new();
    }
}
