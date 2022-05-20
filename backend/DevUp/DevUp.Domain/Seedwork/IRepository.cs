using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevUp.Domain.Seedwork
{
    public interface IRepository<TEntity, TEntityId>
        where TEntity : Entity<TEntityId>
        where TEntityId : EntityId
    {
        Task<TEntity> GetByIdAsync(TEntityId id, CancellationToken cancellationToken);
        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    }
}
