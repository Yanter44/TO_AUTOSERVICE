namespace ToMainApi.Models.Entities
{
    public class Pto
    {
        public int Id { get; set; }

        //LegalInfo
        public string Name { get; set; }
        public string RsaNumber { get; set; }
        public string Address { get; set; }
        //Geolocation
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        //IntergrationWithEAI
        public string Login { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public List<PtoPricePolicy> PricePolicies { get; set; }
    }
}
