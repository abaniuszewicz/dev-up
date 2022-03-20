using System.Threading.Tasks;

namespace DevUp.Infrastructure.Persistence
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
