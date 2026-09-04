using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.Prompt;
using ToMainApi.Models.Dtos.User;
using ToMainApi.Models.Entities;

namespace ToMainApi.Services
{
    public class PromptService : IPromptService
    {
        private readonly AppDbContext _dbcontext;
        public PromptService(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<ServiceResponse<List<PromptDto>>> GetAllPrompts(UserContextDto userContext)
        {
           var prompts = await _dbcontext.Prompts.AsNoTracking()
                .Where(x => x.UserId == userContext.Id)
                .Select(x => new PromptDto
                {
                   PromptId = x.Id,
                   Tag = x.Tag,
                   Description = x.Description
                })
                .ToListAsync();
            return new ServiceResponse<List<PromptDto>>
            {
                Data = prompts,
                Success = true
            };
        }

        public async Task<ServiceResponse<PagedResponse<PromptDto>>> GetPrompts(UserContextDto userContext,
           PaginationDto paginationModel)
        {
            var query = _dbcontext.Prompts
                .AsNoTracking()
                .Where(x => x.UserId == userContext.Id)
                .Select(x => new PromptDto
                {
                    PromptId = x.Id,
                    Tag = x.Tag,
                    Description = x.Description
                });

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((paginationModel.Page - 1) * paginationModel.PageSize)
                .Take(paginationModel.PageSize)
                .ToListAsync();

            return new ServiceResponse<PagedResponse<PromptDto>>
            {
                Data = new PagedResponse<PromptDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = paginationModel.Page,
                    PageSize = paginationModel.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / paginationModel.PageSize)
                },
                Success = true
            };
        }
        public async Task<ServiceResponse<List<string>>> GetPromptsByUserIdAndIds(int userId,List<int> promptsIds)
        {
            var prompts = await _dbcontext.Prompts
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Where(p => promptsIds.Contains(p.Id))
                .Select(p => p.Description)
                .ToListAsync();

            return new ServiceResponse<List<string>>()
            {
                Data = prompts,
                Success = true
            };
        }
        public async Task<ServiceResponse<string>> GetPromptByUserId(int userId, int promptId)
        {
            var prompt = await _dbcontext.Prompts
                .AsNoTracking()
                .Where(p => p.Id == promptId)
                .Where(p => p.UserId == userId)
                .Select(p => p.Description)
                .FirstOrDefaultAsync();

            if (prompt == null)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Data = null,
                    Message = "Промпт не найден"
                };
            }

            return new ServiceResponse<string>
            {
                Success = true,
                Data = prompt
            };
        }
        public async Task<ServiceResponse<PromptDto>> AddNewPromptAsync(int UserId, AddNewPromptDto model)
        {
            var entity = new Prompt
            {
                UserId = UserId,
                Tag = model.Tag,
                Description = model.Description
            };

            await _dbcontext.Prompts.AddAsync(entity);
            await _dbcontext.SaveChangesAsync();

            var dto = new PromptDto
            {
                PromptId = entity.Id,
                Tag = entity.Tag,
                Description = entity.Description
            };

            return new ServiceResponse<PromptDto>
            {
                Data = dto,
                Success = true,
                Message = "Промпт успешно добавлен"
            };
        }
        public async Task<ServiceResponse<bool>> DeletePromptAsync(int UserId, int promptId)
        {
            var prompt = await _dbcontext.Prompts
                .FirstOrDefaultAsync(x => x.Id == promptId && x.UserId == UserId);

            if (prompt == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Prompt not found"
                };
            }

            _dbcontext.Prompts.Remove(prompt);
            await _dbcontext.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true
            };
        }
        public async Task<ServiceResponse<PromptDto>> UpdatePromptAsync(int UserId, UpdatePromptDto model)
        {
            var prompt = await _dbcontext.Prompts
                .FirstOrDefaultAsync(x => x.Id == model.PromptId && x.UserId == UserId);

            if (prompt == null)
            {
                return new ServiceResponse<PromptDto>
                {
                    Data = null,
                    Success = false,
                    Message = "Prompt not found"
                };
            }

            prompt.Tag = model.Tag;
            prompt.Description = model.Description; 

            _dbcontext.Prompts.Update(prompt);
            await _dbcontext.SaveChangesAsync();

            var dto = new PromptDto
            {
                PromptId = prompt.Id,
                Tag = prompt.Tag,
                Description = prompt.Description
            };

            return new ServiceResponse<PromptDto>
            {
                Data = dto,
                Success = true,
                Message = "Промпт успешно обновлен"
            };
        }
    }
}
