namespace ToMainApi.Models.Entities
{
    public class VehicleData
    {
        public string Id { get; set; }
        public string GosNumber { get; set; }
        public string Mark { get; set; }
        public string Model { get; set; }
        public int VehicleCategoryId { get; set; }
        public VehicleCategory Category { get; set; }
        public int YearOfRelease { get; set; }
    }
}
