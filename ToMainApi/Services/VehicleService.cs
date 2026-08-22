using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Vehicle;
using ToMainApi.Models.Entities;

public class VehicleService : IVehicleService
{
    private readonly AppDbContext _dbcontext;

    public VehicleService(AppDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    public async Task<ServiceResponse<List<VehicleCategoryDto>>> GetAllVehicleCategories()
    {
        var result = await _dbcontext.VehicleCategories.ToListAsync();

        if (result != null && result.Count > 0)
        {
            var vehicleCategoryDtos = result.Select(res => new VehicleCategoryDto
            {
                Id = res.Id,
                Name = res.Name
            }).ToList();

            return new ServiceResponse<List<VehicleCategoryDto>>
            {
                Data = vehicleCategoryDtos,
                Success = true
            };
        }

        return new ServiceResponse<List<VehicleCategoryDto>>
        {
            Success = true,
            Message = "Категорий не найдено",
            Data = new List<VehicleCategoryDto>()
        };
    }
    public async Task<ServiceResponse<bool>> AddNewVehicleCategory(AddNewVehicleCategoryDto model)
    {
        var exists = await _dbcontext.VehicleCategories
            .AnyAsync(x => x.Name.ToLower() == model.Name.Trim().ToLower());

        if (exists)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Категория с таким названием уже существует",
                Data = false
            };
        }
        var newCategory = new VehicleCategory
        {
            Name = model.Name.Trim()
        };
        _dbcontext.VehicleCategories.Add(newCategory);
        await _dbcontext.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Success = true,
            Message = "Категория успешно добавлена",
            Data = true
        };
    }
    public async Task<ServiceResponse<bool>> UpdateVehicleCategory(UpdateVehicleCategoryDto model)
    {
        var category = await _dbcontext.VehicleCategories.FindAsync(model.Id);

        if (category == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Категория для обновления не найдена",
                Data = false
            };
        }
        var nameOccupied = await _dbcontext.VehicleCategories
            .AnyAsync(x => x.Name.ToLower() == model.Name.Trim().ToLower() && x.Id != model.Id);

        if (nameOccupied)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Другая категория уже использует это название",
                Data = false
            };
        }
        category.Name = model.Name.Trim();
        _dbcontext.VehicleCategories.Update(category);
        await _dbcontext.SaveChangesAsync();

        return new ServiceResponse<bool>
        {
            Success = true,
            Message = "Категория успешно обновлена",
            Data = true
        };
    }
    public async Task<ServiceResponse<bool>> DeleteVehicleCategory(DeleteVehicleCategoryDto model)
    {
        var category = await _dbcontext.VehicleCategories.FindAsync(model.Id);

        if (category == null)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Message = "Категория для удаления не найдена",
                Data = false
            };
        }
        _dbcontext.VehicleCategories.Remove(category);
        await _dbcontext.SaveChangesAsync();
        return new ServiceResponse<bool>
        {
            Success = true,
            Message = "Категория успешно удалена",
            Data = true
        };
    }
}