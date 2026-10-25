using ToMainApi.Models.Enums;

namespace ToMainApi.Models.Entities
{
    public class Application
    {
        public int Id { get; set; }

        //AgentData
        public int AgentId { get; set; }
        public AgentProfile Agent { get; set; }

        //Provided_Document
        public int ProvidedDocumentId { get; set; }
        public ApplicationProvidedDocument ProvidedDocument { get; set; }
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

        public int VehicleCategoryBySrtsId { get; set; }
        public VehicleCategoryBySrts VehicleCategoryBySrts { get; set; }
        public int VehicleCategoryByOkpId { get; set; }
        public VehicleCategoryByOkp VehicleCategoryByOkp { get; set; }

        public int VehicleMileage { get; set; }
        public int AlowedMaximumMass { get; set; }
        public string FuelType { get; set; }
        public string TypeOfBrakeSystemDrive { get; set; }
        public string TireBrand { get; set; }
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
        public DateOnly? DateOfTheLastExaminationGasCylinder { get; set; }
        public DateOnly? DateOfTheNextInspectionGasCylinder { get; set; }

        // OwnerData
        public string FIO { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // Photos 
        public List<ApplicationPhoto> Photos { get; set; }

        // PTO
        public int PtoId { get; set; }
        public Pto Pto { get; set; }


    //    public int SelectedTechnicalExpertToReviewApplicationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string? RejectReason { get; set; }
    }
}
