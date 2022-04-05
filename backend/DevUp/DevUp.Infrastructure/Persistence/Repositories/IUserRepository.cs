using DevUp.Domain.Entities.Identity;

namespace DevUp.Infrastructure.Persistence.Repositories
{
    public interface IUserRepository : IRepository<User, UserId>
    {
    }
}
