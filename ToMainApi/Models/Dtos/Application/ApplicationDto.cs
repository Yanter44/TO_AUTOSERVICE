using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Dtos.Application
{
    public class ApplicationDto
    {
        public int Id { get; set; }

        public string ProvidedDocumentName{ get; set; }
        public string ProvidedDocumentSeriesAndNumber { get; set; }
        public DateOnly ProvidedDocumentWhenWasIssued { get; set; }
        public string ProvidedDocumentIssuedBy { get; set; }

        //VehicleData
        public bool IsVinMissing { get; set; }
        public string? VIN { get; set; }
        public int VehicleYearOfRelease { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string GosNumber { get; set; }
        public string VehicleBodyNumber { get; set; }
        public bool IsChassisNumberMissing { get; set; }
        public string? ChassisNumber { get; set; }
        public int? AdditionalMarking { get; set; }
        public int MassWithoutLoad { get; set; }
        public string VehicleCategoryBySrts{ get; set; }
        public string VehicleCategoyByOkp { get; set; }
        public int VehicleMileage { get; set; }
        public int AlowedMaximumMass { get; set; }
        public string FuelType { get; set; }
        public string TypeOfBrakeSystemDrive { get; set; }
        public string TireBrand { get; set; }
        public bool IsSpecialPurposeMissing { get; set; }
        public string? SpecialPurpose { get; set; }

        //Tahogram
        public bool IsTahogramMissing { get; set; }
        public string? BrandOfTachograph { get; set; }
        public string? ModelOfTachograph { get; set; }
        public string? SerialNumberOfTachograph { get; set; }

        //GasCylinderEquipment
        public bool IsGasCylinderEquipmentMissing { get; set; }
        public string? NumberOfGasCylinderEquipment { get; set; }
        public DateOnly? DateOfTheNextInspectionGasCylinderEquipment { get; set; }

        //GasСylinder
        public bool IsGasCylinderMissing { get; set; }
        public int? GasCylinderYearOfRelease { get; set; }
        public string? GasCylinderSerialNumber { get; set; }
        public DateOnly? DateOfTheLastInspectionGasCylinder { get; set; }
        public DateOnly? DateOfTheNextInspectionGasCylinder { get; set; }

        // OwnerData
        public string FIO { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // Photos 
        public List<ApplicationPhotoDto> Photos { get; set; }

        // PTO
        public int PtoId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
