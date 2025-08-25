using Microsoft.EntityFrameworkCore;

namespace ITC.Authorization.Storage;

public class ServiceDbContext : DbContext
{
    public ServiceDbContext(DbContextOptions<ServiceDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InitialBrokerRegistrationRequest>(entity =>
        {
            entity.HasQueryFilter(c => !c.DeletedAt.HasValue);
        });
        modelBuilder.Entity<UserRolesCache>(entity =>
        {
            entity.HasQueryFilter(c => !c.DeletedAt.HasValue);
        });
        modelBuilder.Entity<InvitedUser>(entity =>
        {
            entity.HasQueryFilter(c => !c.DeletedAt.HasValue);
        });
        modelBuilder.Entity<ExternalAuthToken>(entity =>
        {
            entity.HasQueryFilter(c => !c.DeletedAt.HasValue);
        });
        modelBuilder.Entity<PasswordResetUser>(entity =>
        {
            entity.HasQueryFilter(c => !c.DeletedAt.HasValue);
        });
    }
}