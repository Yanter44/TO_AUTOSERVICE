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
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        //IntergrationWithEAI
        public int MaximumThroughput { get; set; }
        public IFormFile OperatorKeyArchive { get; set; }
        public List<AddNewPtoExpertDto> Experts { get; set; } = new();

        //PricePolicies
        public List<PtoPricePolicyDto> PricePolicies { get; set; }
    }
}
