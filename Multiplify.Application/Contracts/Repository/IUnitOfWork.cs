namespace Multiplify.Application.Contracts.Repository;
public interface IUnitOfWork
{
    IGenericRepository<T> GetRepository<T>() where T : class;
    Task<bool> SaveChangesAsync();

}