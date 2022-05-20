using DevUp.Domain.Identity.Entities;

namespace DevUp.Infrastructure.Postgres.Repositories
{
    internal class PostgresUserRepository : IUserStore
    {
        public PostgresUserRepository()
        {

        }

        public Task<User> AddAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByIdAsync(UserId id)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
