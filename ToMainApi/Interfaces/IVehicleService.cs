using ToMainApi.Common;
using ToMainApi.Models.Dtos.Vehicle;

namespace ToMainApi.Interfaces
{
    public interface IVehicleService
    {
        Task<ServiceResponse<List<VehicleCategoryDto>>> GetAllVehicleCategories();
        Task<ServiceResponse<bool>> AddNewVehicleCategory(AddNewVehicleCategoryDto model);
        Task<ServiceResponse<bool>> DeleteVehicleCategory(DeleteVehicleCategoryDto model);
        Task<ServiceResponse<bool>> UpdateVehicleCategory(UpdateVehicleCategoryDto model);
    }
}
