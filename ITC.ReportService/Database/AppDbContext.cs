using ITC.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ITC.ReportService.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
        
    public DbSet<Engine> Engines { get; set; }
    public DbSet<Analysis> Analyses { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Engine>(entity =>
        {
            entity.HasQueryFilter(c => !c.DeletedAt.HasValue);
        });
        modelBuilder.Entity<Analysis>(entity =>
        {
            entity.HasQueryFilter(c => !c.DeletedAt.HasValue);
        });
    }
}