using Microsoft.EntityFrameworkCore;

namespace Persistence;

internal class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
    }
}
