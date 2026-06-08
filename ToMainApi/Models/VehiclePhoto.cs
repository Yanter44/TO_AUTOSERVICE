using ToMainApi.Models.Enums;

namespace ToMainApi.Models
{
    public class VehiclePhoto
    {
        public VehiclePhotoType VehiclePhotoType { get; set; }
        public IFormFile Photo { get; set; }
    }
}
