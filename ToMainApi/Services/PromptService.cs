using ToMainApi.DbContext;
using ToMainApi.Interfaces;

namespace ToMainApi.Services
{
    public class PromptService : IPromptService
    {
        private readonly AppDbContext _dbcontext;
        public PromptService(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }


    }
}
