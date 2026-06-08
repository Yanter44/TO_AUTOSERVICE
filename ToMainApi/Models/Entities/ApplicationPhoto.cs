using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Entities
{
    public class ApplicationPhoto
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }
        public Application Application { get; set; }

        public VehiclePhotoType VehiclePhotoType { get; set; }

        public string Url { get; set; }   
    }
}
