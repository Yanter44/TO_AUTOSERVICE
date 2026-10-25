namespace ToMainApi.Models.Entities
{
    public class PtoExpertCryptoKey
    {
        public int Id { get; set; }
        public int ExpertId { get; set; }
        public PtoExpert Expert { get; set; } = null!;

        public byte[] EncryptedData { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
