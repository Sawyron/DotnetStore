using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Persistence.UnitTests;

internal class SqliteContextFactory : IDisposable
{
    private DbConnection? _connection;

    public ApplicationContext CreateContext()
    {
        if (_connection is null)
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            var options = CreateOptions();
            using var context = new ApplicationContext(options);
            context.Database.EnsureCreated();
        }
        return new ApplicationContext(CreateOptions());
    }

    public void Dispose()
    {
        if (_connection != null)
        {
            _connection.Dispose();
            _connection = null;
        }
    }

    private DbContextOptions<ApplicationContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<ApplicationContext>()
            .UseSqlite(_connection!).Options;
    }
}
