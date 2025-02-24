using Microsoft.EntityFrameworkCore;
using SeniorBackend.Core.Entities;
using SeniorBackend.Core.Interfaces.Repositories;
using SeniorBackend.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorBackend.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly DbSet<User> _users;
        private readonly ApplicationDbContext _dbContext;
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _users = dbContext.Set<User>();
            _dbContext = dbContext;
        }


        public async Task<User> GetByIdAsync(string id)
        {
            User user = await _users.FirstOrDefaultAsync(r => r.Id == id);
            if (user == null) throw new Exception("User Not Found");
            return user;
        }
    }
}
