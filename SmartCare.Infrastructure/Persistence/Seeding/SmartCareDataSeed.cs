using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SmartCare.Core.Entities.Identity;

namespace SmartCare.Infrastructure.Persistence.Seeding;

public static class SmartCareDataSeed
{
    public static async Task SeedAsync( SmartCareDbContext context,IServiceProvider services)
    {
        await SeedRolesAsync(services);

        await SeedAdminAsync(services);

        // Later:
        // await SeedSpecialtiesAsync(context);
        // await SeedClinicsAsync(context);
        // await SeedDoctorsAsync(context);
    }

    private static async Task SeedRolesAsync(
        IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        string[] roles = [ "Admin","Doctor","Patient","Receptionist"];
        
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }

    private static async Task SeedAdminAsync( IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        const string    adminEmail = "Mohamed@gmail.com";

        var existingAdmin =await userManager.FindByEmailAsync(adminEmail);

        if (existingAdmin is not null)
            return;

        var admin = new ApplicationUser
        {
            FirstName = "Mohamed",
            LastName = "Admin",
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(admin,"P@ssw0rd!");

        if (!result.Succeeded)
        {
            var errors = string.Join( ", ", result.Errors.Select(x => x.Description));

            throw new Exception( $"Failed to create admin user: {errors}");
        }

        await userManager.AddToRoleAsync( admin,"Admin");
    }
}