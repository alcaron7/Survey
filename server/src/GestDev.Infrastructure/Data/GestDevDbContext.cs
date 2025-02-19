using GestDev.Core.Entities;
using Microsoft.EntityFrameworkCore;

public class GestDevDbContext : DbContext
{

    public DbSet<User> Users { get; set; }

    public GestDevDbContext(DbContextOptions<GestDevDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GestDevDbContext).Assembly);
    }
}