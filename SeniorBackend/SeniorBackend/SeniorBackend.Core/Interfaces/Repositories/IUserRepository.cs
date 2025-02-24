using SeniorBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorBackend.Core.Interfaces.Repositories
{
    public interface IUserRepository:IGenericRepository<User>
    {
        public  Task<User> GetByIdAsync(string id);
    }
}
