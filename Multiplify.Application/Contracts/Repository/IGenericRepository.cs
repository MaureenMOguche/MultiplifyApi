using System.Linq.Expressions;

namespace Multiplify.Application.Contracts.Repository;
public interface IGenericRepository<T> where T : class
{
    IQueryable<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool trackChanges = false);
    Task<T> AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task AddBulkAsync(IEnumerable<T> entities);
    void DeleteBulk(IEnumerable<T> entities);
}
