using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Repositories
{
    public interface IUserRepository : IRepository<User, UserId>
    {
    }
}
