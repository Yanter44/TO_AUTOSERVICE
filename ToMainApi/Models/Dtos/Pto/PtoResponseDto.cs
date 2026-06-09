namespace ToMainApi.Models.Dtos.Pto
{
    public class PtoResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string RsaNumber { get; set; }
        public string Address { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public List<PtoPricePolicyDto> PricePolicies { get; set; }
    }
}
