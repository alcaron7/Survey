using Survey.Core.Entities;
using Microsoft.EntityFrameworkCore;

public class SurveyDbContext : DbContext
{

    public DbSet<User> Users { get; set; }

    public SurveyDbContext(DbContextOptions<SurveyDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SurveyDbContext).Assembly);
    }
}