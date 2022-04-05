using System.Threading.Tasks;
using DevUp.Infrastructure.Persistence.Repositories;

namespace DevUp.Infrastructure.Persistence
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepository { get; }

        Task SaveChangesAsync();
    }
}
