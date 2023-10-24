namespace FC.Codeflix.Catalog.Application.Interfaces;
public interface IUnitOfWork
{
    public Task<bool> Commit(CancellationToken cancellationToken);
}
