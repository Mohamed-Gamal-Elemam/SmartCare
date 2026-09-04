
using Hangfire;
using Microsoft.AspNetCore.Identity;
using SmartCare.API.ExceptionHandlers;
using SmartCare.API.Extensions;
using SmartCare.Core.Entities.Identity;
using SmartCare.Infrastructure;
using SmartCare.Infrastructure.Persistence;
using SmartCare.Services.DependencyInjection;

namespace SmartCare.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<SmartCareDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddJwtAuthentication(
            builder.Configuration);

        builder.Services.AddApplicationServices(builder.Configuration);

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.AddProblemDetails();

        builder.Services.AddHangfire(config => config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddHangfireServer();

        var app = builder.Build();

        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseHangfireDashboard();

        await app.SeedDatabaseAsync();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwagger();

            app.UseSwaggerUI();
        }

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}