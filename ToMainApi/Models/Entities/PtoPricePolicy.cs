namespace ToMainApi.Models.Entities
{
    public class PtoPricePolicy
    {
        public int Id { get; set; }

        public int PtoId { get; set; }
        public Pto Pto { get; set; }

        public int VehicleCategoryId { get; set; }
        public VehicleCategory VehicleCategory { get; set; }

        public decimal Price { get; set; }
    }
}
