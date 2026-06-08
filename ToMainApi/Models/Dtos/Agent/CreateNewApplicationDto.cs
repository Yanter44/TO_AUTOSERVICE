using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Dtos.Agent
{
    public class CreateNewApplicationDto
    {
        //VehicleData
        public int VehicleCategoryId { get; set; }
        public string VIN { get; set; }
        public string GosNumber { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int YearOfRelease { get; set; }

        //Documents
        public List<DocumentFiles> DocumentFiles { get; set; }

        //OwnerData
        public string FIO { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        //VehiclePhotos
        public List<VehiclePhoto> VehiclePhotos { get; set; }
        //PTO
        public int PtoId { get; set; }
        
    }
}
