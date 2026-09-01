using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartCare.Core.Entities.Identity;

namespace SmartCare.Infrastructure.Persistence;

public class SmartCareDbContext: IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public SmartCareDbContext(
        DbContextOptions<SmartCareDbContext> options)
        : base(options)
    {
    }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(SmartCareDbContext).Assembly);
    }
}