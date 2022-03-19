using DevUp.Domain.Entities;

namespace DevUp.Repository.Repositories
{
    public interface IRepository<T, T1> where T : class
    {
        Task<T> GetByIdAsync(IEntityId<T1> id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T1> DeleteAsync(IEntityId<T1> id);
    }
} 