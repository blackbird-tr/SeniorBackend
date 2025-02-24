
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SeniorBackend.Core.Interfaces.Repositories;
using SeniorBackend.Infrastructure.Context;
using SeniorBackend.Infrastructure.Repositories;
using System.Reflection;

namespace SeniorBackend.Infrastructure
{
    public static class ServiceRegistration
    {

        public static void AddPersistenceInfrastructure(this IServiceCollection services)
        {
            if (true)
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("ApplicationDb"));
            }
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IUserRepository, UserRepository>();
        }

        }
    }
