namespace ToMainApi.Models.Entities
{
    public class PtoOperatorCryptoKey
    {
        public int PtoId { get; set; }              
        public Pto Pto { get; set; } = null!;

        public byte[] EncryptedData { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
