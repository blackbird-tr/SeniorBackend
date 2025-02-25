
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeniorBackend.Core.Interfaces.Repositories;
using SeniorBackend.Infrastructure.Context;
using SeniorBackend.Infrastructure.Repositories;
using System.Reflection; 
namespace SeniorBackend.Infrastructure
{
    public static class ServiceRegistration
    {

        public static void AddPersistenceInfrastructure(this IServiceCollection services,IConfiguration configuration )
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("ApplicationDb"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                      options.UseSqlServer(
                   configuration.GetConnectionString("DefaultConnection"),
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            }
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IUserRepository, UserRepository>();
        }

        }
    }
