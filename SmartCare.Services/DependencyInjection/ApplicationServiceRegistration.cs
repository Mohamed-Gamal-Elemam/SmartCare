using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartCare.Services.Abstraction.Options;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Services.Mapping;
using SmartCare.Services.Services;

namespace SmartCare.Services.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MailSettingsOptions>(
            configuration.GetSection("MailSettings"));

        services.Configure<JwtOptions>(
            configuration.GetSection("Jwt"));

        services.Configure<AppBaseUrlOptions>(
            configuration.GetSection(AppBaseUrlOptions.SectionName));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailJobService, EmailJobService>();
        services.AddAutoMapper(cfg => { }, typeof(ServicesReference).Assembly);
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ISpecialtyService, SpecialtyService>();

        return services;
    }
}