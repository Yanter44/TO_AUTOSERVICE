using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Dtos.Vehicle
{
    public class AddNewVehicleCategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string VehicleCategoryType { get; set; }
    }
}
