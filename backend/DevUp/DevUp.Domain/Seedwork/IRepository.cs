using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevUp.Domain.Seedwork
{
    public interface IRepository<TEntity, TEntityId>
        where TEntity : Entity<TEntityId>
        where TEntityId : EntityId
    {
        Task<TEntity> GetByIdAsync(TEntityId id);
        Task<IReadOnlyList<TEntity>> GetAllAsync();
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
