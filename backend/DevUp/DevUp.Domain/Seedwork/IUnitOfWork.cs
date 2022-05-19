using System.Threading.Tasks;

namespace DevUp.Domain.Seedwork
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
