using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Entities
{
    public class Application
    {
        public int Id { get; set; }

        // VehicleData
        public int VehicleCategoryId { get; set; }
        public VehicleCategory VehicleCategory { get; set; }
        public string VIN { get; set; }
        public string GosNumber { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int YearOfRelease { get; set; }

        // Documents
        public List<ApplicationDocument> Documents { get; set; }

        // OwnerData
        public string FIO { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // Photos 
        public List<ApplicationPhoto> Photos { get; set; }

        // PTO
        public int PtoId { get; set; }
        public Pto Pto { get; set; }

        public DateTime CreatedAt { get; set; }
        public ApplicationStatus Status { get; set; }
    }
}
