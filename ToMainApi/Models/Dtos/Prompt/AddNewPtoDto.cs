using ToMainApi.Models.Dtos.Pto;

namespace ToMainApi.Models.Dtos.Prompt
{
    public class AddNewPtoDto
    {
        //LegalInfo
        public string Name { get; set; }
        public string RsaNumber { get; set; }
        public string Address { get; set; }
        //Geolocation
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        //IntergrationWithEAI
        public string Login { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
        public List<PtoPricePolicyDto> PricePolicies { get; set; }
    }
}
