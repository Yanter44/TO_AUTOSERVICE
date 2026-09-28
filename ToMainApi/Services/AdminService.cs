using Microsoft.EntityFrameworkCore;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Admin;
using ToMainApi.Models.Dtos.Agent;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _dbcontext;
        public AdminService(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
     
    }
}
