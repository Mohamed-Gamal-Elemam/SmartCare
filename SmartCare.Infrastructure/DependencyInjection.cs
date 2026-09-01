using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCare.Core.Entities.Identity;
using SmartCare.Core.Interfaces;
using SmartCare.Core.Interfaces.Repositories;
using SmartCare.Infrastructure.Authentication;
using SmartCare.Infrastructure.Persistence;
using SmartCare.Infrastructure.Persistence.Repositories;
using SmartCare.Services.Abstraction.Options;
using SmartCare.Services.Abstraction.Services;


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
        
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<ITokenService, TokenService>();



        return services;
    }
}