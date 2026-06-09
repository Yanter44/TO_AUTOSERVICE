using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Dtos.Application
{
    public class ApplicationPhotoDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public VehiclePhotoType VehiclePhotoType { get; set; }
        public string Url { get; set; }

    }
}
