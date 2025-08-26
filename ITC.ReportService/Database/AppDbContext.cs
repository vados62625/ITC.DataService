using Microsoft.EntityFrameworkCore;

namespace ITC.ReportService.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<Domain.Models.Engine> Engines { get; set; }
        public DbSet<Domain.Models.Analysis> Analyses { get; set; }
    }
}