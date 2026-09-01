using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCare.Core.Entities.Identity;
using SmartCare.Core.Interfaces;
using SmartCare.Core.Interfaces.Repositories;
using SmartCare.Infrastructure.Persistence;
using SmartCare.Infrastructure.Persistence.Repositories;


namespace SmartCare.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<SmartCareDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped(typeof(IGenericRepository<,>),typeof(GenericRepository<,>));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        




        return services;
    }
}