using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Vehicle;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly AppDbContext _db;
        public VehicleService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<ServiceResponse<List<VehicleCategoryDto>>> GetAllVehicleCategories(
            VehicleCategoryType? type = null)
        {
            var result = new List<VehicleCategoryDto>();

            if (type is null or VehicleCategoryType.SRTS)
            {
                var srts = await _db.VehicleCategoriesBySrts
                    .Select(x => new VehicleCategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Type = nameof(VehicleCategoryType.SRTS)
                    })
                    .ToListAsync();

                result.AddRange(srts);
            }

            if (type is null or VehicleCategoryType.OKP)
            {
                var okp = await _db.VehicleCategoriesByOkp
                    .Select(x => new VehicleCategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Type = nameof(VehicleCategoryType.OKP)
                    })
                    .ToListAsync();

                result.AddRange(okp);
            }

            return new ServiceResponse<List<VehicleCategoryDto>>
            {
                Success = true,
                Data = result,
                Message = result.Count > 0 ? null : "Категорий не найдено"
            };
        }

        public async Task<ServiceResponse<bool>> AddNewVehicleCategory(AddNewVehicleCategoryDto model)
        {
            if (!Enum.TryParse<VehicleCategoryType>(model.VehicleCategoryType, ignoreCase: true, out var type)
                || !Enum.IsDefined(type))
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Неизвестный тип категории. Допустимые: SRTS, OKP"
                };

            var name = model.Name.Trim();
            var description = model.Description?.Trim();

            var exists = type switch
            {
                VehicleCategoryType.SRTS => await _db.VehicleCategoriesBySrts
                    .AnyAsync(x => x.Name.ToLower() == name.ToLower()),
                VehicleCategoryType.OKP => await _db.VehicleCategoriesByOkp
                    .AnyAsync(x => x.Name.ToLower() == name.ToLower()),
                _ => false
            };

            if (exists)
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Категория с таким названием уже существует"
                };

            switch (type)
            {
                case VehicleCategoryType.SRTS:
                    _db.VehicleCategoriesBySrts.Add(new VehicleCategoryBySrts
                    {
                        Name = name,
                        Description = description
                    });
                    break;

                case VehicleCategoryType.OKP:
                    _db.VehicleCategoriesByOkp.Add(new VehicleCategoryByOkp
                    {
                        Name = name,
                        Description = description
                    });
                    break;
            }

            await _db.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Success = true,
                Message = ResponseMessages.Success,
                Data = true
            };
        }

        public async Task<ServiceResponse<bool>> UpdateVehicleCategory(UpdateVehicleCategoryDto model)
        {
            if (!Enum.TryParse<VehicleCategoryType>(model.VehicleCategoryType, ignoreCase: true, out var type)
                || !Enum.IsDefined(type))
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Неизвестный тип категории. Допустимые: SRTS, OKP"
                };

            var name = model.Name.Trim();
            var description = model.Description?.Trim();

            switch (type)
            {
                case VehicleCategoryType.SRTS:
                    {
                        var category = await _db.VehicleCategoriesBySrts.FindAsync(model.Id);
                        if (category == null)
                            return new ServiceResponse<bool>
                            {
                                Success = false,
                                Message = "Категория для обновления не найдена"
                            };

                        var occupied = await _db.VehicleCategoriesBySrts
                            .AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != model.Id);
                        if (occupied)
                            return new ServiceResponse<bool>
                            {
                                Success = false,
                                Message = "Другая категория уже использует это название"
                            };

                        category.Name = name;
                        category.Description = description;
                        break;
                    }
                case VehicleCategoryType.OKP:
                    {
                        var category = await _db.VehicleCategoriesByOkp.FindAsync(model.Id);
                        if (category == null)
                            return new ServiceResponse<bool>
                            {
                                Success = false,
                                Message = "Категория для обновления не найдена"
                            };

                        var occupied = await _db.VehicleCategoriesByOkp
                            .AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != model.Id);
                        if (occupied)
                            return new ServiceResponse<bool>
                            {
                                Success = false,
                                Message = "Другая категория уже использует это название"
                            };

                        category.Name = name;
                        category.Description = description;
                        break;
                    }
            }

            await _db.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Success = true,
                Message = ResponseMessages.Success,
                Data = true
            };
        }

        public async Task<ServiceResponse<bool>> DeleteVehicleCategory(DeleteVehicleCategoryDto model)
        {
            if (!Enum.TryParse<VehicleCategoryType>(model.VehicleCategoryType, ignoreCase: true, out var type)
                || !Enum.IsDefined(type))
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Неизвестный тип категории. Допустимые: SRTS, OKP"
                };

            switch (type)
            {
                case VehicleCategoryType.SRTS:
                    {
                        var category = await _db.VehicleCategoriesBySrts.FindAsync(model.Id);
                        if (category == null)
                            return new ServiceResponse<bool>
                            {
                                Success = false,
                                Message = "Категория для удаления не найдена"
                            };

                        _db.VehicleCategoriesBySrts.Remove(category);
                        break;
                    }
                case VehicleCategoryType.OKP:
                    {
                        var category = await _db.VehicleCategoriesByOkp.FindAsync(model.Id);
                        if (category == null)
                            return new ServiceResponse<bool>
                            {
                                Success = false,
                                Message = "Категория для удаления не найдена"
                            };

                        _db.VehicleCategoriesByOkp.Remove(category);
                        break;
                    }
            }

            await _db.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Success = true,
                Message = ResponseMessages.Success,
                Data = true
            };
        }
    }
}