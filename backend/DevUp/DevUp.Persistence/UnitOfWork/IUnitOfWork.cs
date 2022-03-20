using System.Threading.Tasks;

namespace DevUp.Persistence.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
