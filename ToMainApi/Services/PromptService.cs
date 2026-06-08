using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos;
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

        public async Task<ServiceResponse<List<PromtDtoRequest>>> GetAllPrompts(int UserId)
        {
            var prompts = await _dbcontext.Prompts.Where(x => x.UserId == UserId)
                                                  .Select(x => new PromtDtoRequest
                                                  {
                                                      Tag = x.Tag,
                                                      Description = x.Description
                                                  })
                                                 .ToListAsync();
            return new ServiceResponse<List<PromtDtoRequest>>
            {
                Data = prompts,
                Success = true
            };
        }

        public async Task<ServiceResponse<bool>> AddNewPromptAsync(int UserId, AddNewPromptDto model)
        {
            var entity = new Prompt
            {
                UserId = UserId,
                Tag = model.Tag,
                Description = model.Description
            };

            await _dbcontext.Prompts.AddAsync(entity);
            await _dbcontext.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true
            };
        }
        public async Task<ServiceResponse<bool>> DeletePromptAsync(int UserId, DeletePromptDto model)
        {
            var prompt = await _dbcontext.Prompts
                .FirstOrDefaultAsync(x => x.Id == model.PromtId && x.UserId == UserId);

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
        public async Task<ServiceResponse<bool>> UpdatePromptAsync(int UserId, UpdatePromptDto model)
        {
            var prompt = await _dbcontext.Prompts
                .FirstOrDefaultAsync(x => x.Id == model.PromptId && x.UserId == UserId);

            if (prompt == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Prompt not found"
                };
            }

            prompt.Tag = model.Tag;
            prompt.Description = model.Descripion;

            _dbcontext.Prompts.Update(prompt);
            await _dbcontext.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true
            };
        }
    }
}
