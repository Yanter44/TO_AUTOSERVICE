using ToMainApi.Common;
using ToMainApi.Models.Dtos.Vehicle;
using ToMainApi.Models.Enums;

namespace ToMainApi.Interfaces
{
    public interface IVehicleService
    {
        Task<ServiceResponse<List<VehicleCategoryDto>>> GetAllVehicleCategories(VehicleCategoryType? type = null);

        Task<ServiceResponse<bool>> AddNewVehicleCategory(AddNewVehicleCategoryDto model);
        Task<ServiceResponse<bool>> UpdateVehicleCategory(UpdateVehicleCategoryDto model);
        Task<ServiceResponse<bool>> DeleteVehicleCategory(DeleteVehicleCategoryDto model);
    }
}
