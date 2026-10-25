namespace ToMainApi.Models.Entities
{
    public class PtoExpert
    {
        public int Id { get; set; }
        public int PtoId { get; set; }                
        public Pto Pto { get; set; } = null!;

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public int SequenceNumber { get; set; }

        public PtoExpertCryptoKey? CryptoKey { get; set; }   // 1:1 навигация
    }
}
