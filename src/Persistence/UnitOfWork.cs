using Application.Core;

namespace Persistence;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _applicationContext;

    public UnitOfWork(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _applicationContext.SaveChangesAsync(cancellationToken);
}
