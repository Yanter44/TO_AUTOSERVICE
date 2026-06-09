using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Dtos.Application
{
    public class ApplicationDto
    {
        public int Id { get; set; }

        // VehicleData
        public int VehicleCategoryId { get; set; }
        public string VIN { get; set; }
        public string GosNumber { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int YearOfRelease { get; set; }

        // Documents
        public List<ApplicationDocumentDto> Documents { get; set; }

        // OwnerData
        public string FIO { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // Photos 
        public List<ApplicationPhotoDto> Photos { get; set; }

        // PTO
        public int PtoId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ApplicationStatus Status { get; set; }
    }
}
