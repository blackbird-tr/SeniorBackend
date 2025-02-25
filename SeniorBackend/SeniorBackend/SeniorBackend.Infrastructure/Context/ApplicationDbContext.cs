using Microsoft.EntityFrameworkCore;
using SeniorBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorBackend.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
          
        }
        public DbSet<User> Users { get; set; }
    }
}
