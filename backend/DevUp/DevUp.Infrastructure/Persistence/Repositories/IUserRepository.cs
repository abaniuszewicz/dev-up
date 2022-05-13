using DevUp.Domain.Identity.Entities;

namespace DevUp.Infrastructure.Persistence.Repositories
{
    public interface IUserRepository : IRepository<User, UserId>
    {
    }
}
