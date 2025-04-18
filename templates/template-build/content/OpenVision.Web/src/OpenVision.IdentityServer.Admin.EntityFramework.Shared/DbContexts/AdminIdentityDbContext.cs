using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenVision.IdentityServer.Admin.EntityFramework.Shared.Configuration.Schema;
using OpenVision.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity;

namespace OpenVision.IdentityServer.Admin.EntityFramework.Shared.DbContexts;

public class AdminIdentityDbContext : IdentityDbContext<UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken>
{
    private readonly IdentityTableConfiguration _schemaConfiguration;

    public AdminIdentityDbContext(DbContextOptions<AdminIdentityDbContext> options, IdentityTableConfiguration schemaConfiguration = null) : base(options)
    {
        _schemaConfiguration = schemaConfiguration ?? new IdentityTableConfiguration();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureIdentityContext(builder);
    }

    private void ConfigureIdentityContext(ModelBuilder builder)
    {
        builder.Entity<UserIdentityRole>().ToTable(_schemaConfiguration.IdentityRoles);
        builder.Entity<UserIdentityRoleClaim>().ToTable(_schemaConfiguration.IdentityRoleClaims);
        builder.Entity<UserIdentityUserRole>().ToTable(_schemaConfiguration.IdentityUserRoles);

        builder.Entity<UserIdentity>().ToTable(_schemaConfiguration.IdentityUsers);
        builder.Entity<UserIdentityUserLogin>().ToTable(_schemaConfiguration.IdentityUserLogins);
        builder.Entity<UserIdentityUserClaim>().ToTable(_schemaConfiguration.IdentityUserClaims);
        builder.Entity<UserIdentityUserToken>().ToTable(_schemaConfiguration.IdentityUserTokens);
    }
}
