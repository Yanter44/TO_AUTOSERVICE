using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;

namespace ToMainApi.Services
{
    public class ModeratorService : IModeratorService
    {
        private readonly AppDbContext _dbcontext;
        public ModeratorService(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

    }
}
