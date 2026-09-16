using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.NeuronNetwork;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.Prompt;
using ToMainApi.Models.Dtos.Pto;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Entities;

namespace ToMainApi.Services
{
    public class NeuronNetworkService : INeuronNetworkService
    {
        private readonly ILogger<NeuronNetworkService> _logger;
        private readonly AppDbContext _dbcontext;
        public NeuronNetworkService(ILogger<NeuronNetworkService> logger,AppDbContext dbcontext)
        {
            _logger = logger;
            _dbcontext = dbcontext;
        }
        
        public async Task<ServiceResponse<NeuronNetworkDto>> AddNewNeuronNetwork(AddNewNeuronNetworkDto model)
        {
            try {
                var existNeuronNetwork = await _dbcontext.RouteAINeuronNetworks.FirstOrDefaultAsync(x => x.Link == model.Link);
                if (existNeuronNetwork != null)
                {
                    return new ServiceResponse<NeuronNetworkDto>() { Data = null, Success = false, Message = "Данная нейронная сеть уже есть в базе данных" };
                }
                var entity = new RouteAINeuronNetwork
                {
                    Name = model.Name,
                    Link = model.Link,
                    CreatedAt = DateTime.UtcNow
                };
                _dbcontext.RouteAINeuronNetworks.Add(entity);
                await _dbcontext.SaveChangesAsync();
                var responsemodel = new NeuronNetworkDto()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Link = entity.Link,
                    CreatedAt = entity.CreatedAt
                };
                return new ServiceResponse<NeuronNetworkDto>() { Data = responsemodel, Success = true, Message = "Нейронная сеть успешно добавлена" };
            }
            catch(Exception ex){
                _logger.LogError(ex, "Ошибка при добавлении новой нейронной сети");
                return new ServiceResponse<NeuronNetworkDto>
                {
                    Data = null,
                    Success = false,
                    Message = ResponseMessages.UnSuccess
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteNeuronNetwork(int NeuronNetworkId)
        {
            try {     
               var existNeuronNetwork = await _dbcontext.RouteAINeuronNetworks
                                            .FirstOrDefaultAsync(x => x.Id == NeuronNetworkId);

                if (existNeuronNetwork == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = "Нейронная сеть не найдена"
                    };
                }
                _dbcontext.RouteAINeuronNetworks.Remove(existNeuronNetwork);
                await _dbcontext.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Success = true
                };
            }
            catch(Exception ex){
                _logger.LogError(ex, "Произошла ошибка при удалении нейронной сети");
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = ResponseMessages.UnSuccess
                };
            }
        }
        public async Task<ServiceResponse<NeuronNetworkDto>> UpdateNeuronNetwork(UpdateNeuronNetworkDto model)
        {
            try {
                var existingNeuronNetwork = await _dbcontext.RouteAINeuronNetworks.FirstOrDefaultAsync(x => x.Id == model.NeuronNetworkId);
                if(existingNeuronNetwork == null){
                    return new ServiceResponse<NeuronNetworkDto>
                    {
                        Data = null,
                        Success = false,
                        Message = "Нейронная сеть не найдена"
                    };
                }
                existingNeuronNetwork.Name = model.Name;
                existingNeuronNetwork.Link = model.Link;
                _dbcontext.RouteAINeuronNetworks.Update(existingNeuronNetwork);
                await _dbcontext.SaveChangesAsync();

                var dto = new NeuronNetworkDto
                {
                    Id = existingNeuronNetwork.Id,
                    Name = existingNeuronNetwork.Name,
                    Link = existingNeuronNetwork.Link,
                    CreatedAt = existingNeuronNetwork.CreatedAt
                };

                return new ServiceResponse<NeuronNetworkDto>
                {
                    Data = dto,
                    Success = true,
                    Message = "Нейронная сеть успешно обновлена"
                };
            }
            catch(Exception ex){
                _logger.LogError(ex, "Произошла ошибка при изменении нейронной сети");
                return new ServiceResponse<NeuronNetworkDto>
                {
                    Data = null,
                    Success = false,
                    Message = ResponseMessages.UnSuccess
                };
            }
        }
        public async Task<ServiceResponse<NeuronNetworkDto>> GetNeuronNetworkById(int neuronNetworkId)
        {
            try
            {
                var existNeuronNetwork = await _dbcontext.RouteAINeuronNetworks.FirstOrDefaultAsync(x => x.Id == neuronNetworkId);
                if(existNeuronNetwork != null)
                {
                    var neuronNetworkDto = new NeuronNetworkDto
                    {
                        Id = existNeuronNetwork.Id,
                        Name = existNeuronNetwork.Name,
                        Link = existNeuronNetwork.Link,
                        CreatedAt = existNeuronNetwork.CreatedAt
                    };
                    return new ServiceResponse<NeuronNetworkDto>() { Data = neuronNetworkDto, Success = true, Message = ResponseMessages.Success };
                }
                return new ServiceResponse<NeuronNetworkDto>() { Data = null, Success = false, Message = ResponseMessages.UnSuccess };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при поиске нейронки по id");
                return new ServiceResponse<NeuronNetworkDto>() { Data = null, Success = false, Message = ResponseMessages.UnSuccess };
            }
        }
        public async Task<ServiceResponse<List<NeuronNetworkDto>>> GetAllNeuronNetworks()
        {
            try
            {
                var result = await _dbcontext.RouteAINeuronNetworks
                    .AsNoTracking()
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new NeuronNetworkDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Link = x.Link,
                        CreatedAt = x.CreatedAt
                    })
                    .ToListAsync();

                return new ServiceResponse<List<NeuronNetworkDto>>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех нейронных сетей");
                return new ServiceResponse<List<NeuronNetworkDto>>
                {
                    Success = false,
                    Data = null,
                    Message = ResponseMessages.UnSuccess
                };
            }
        }
        public async Task<ServiceResponse<PagedResponse<NeuronNetworkDto>>> GetNeuronNetworks(PaginationDto paginationModel)
        {
            try
            {
                var query = _dbcontext.RouteAINeuronNetworks.AsNoTracking()
                .Select(x => new NeuronNetworkDto{
                    Id = x.Id,
                    Name = x.Name,
                    Link = x.Link,
                    CreatedAt = x.CreatedAt
                });
                
                var totalCount = await _dbcontext.RouteAINeuronNetworks.CountAsync();
                var result = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((paginationModel.Page - 1) * paginationModel.PageSize)
                .Take(paginationModel.PageSize)
                .ToListAsync();

                return new ServiceResponse<PagedResponse<NeuronNetworkDto>>
                {
                    Success = true,
                    Data = new PagedResponse<NeuronNetworkDto>
                    {
                        Items = result,
                        TotalCount = totalCount,
                        Page = paginationModel.Page,
                        PageSize = paginationModel.PageSize,
                        TotalPages = (int)Math.Ceiling((double)totalCount / paginationModel.PageSize)
                    }
                };
            }
            catch(Exception ex){
                _logger.LogError(ex, "Ошибка при получении нейронных сетей");
                return new ServiceResponse<PagedResponse<NeuronNetworkDto>>
                {
                    Data = null,
                    Success = false,
                    Message = ResponseMessages.UnSuccess
                };
            }

        }
    }
}