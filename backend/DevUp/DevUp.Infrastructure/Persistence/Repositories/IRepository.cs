using DevUp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevUp.Infrastructure.Persistence.Repositories
{
    public interface IRepository<TEntity, TEntityId> where TEntity : Entity<TEntityId>
    {
        Task<TEntity> GetByIdAsync(EntityId<TEntityId> id);
        Task<IReadOnlyList<TEntity>> GetAllAsync();
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
