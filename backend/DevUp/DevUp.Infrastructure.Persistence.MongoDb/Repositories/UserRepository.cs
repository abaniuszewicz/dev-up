using AutoMapper;
using DevUp.Domain.Entities.Identity;
using DevUp.Infrastructure.Persistence.MongoDb.EntitiesDtos.IdentityDtos;
using DevUp.Infrastructure.Persistence.Repositories;
using MongoDB.Driver;

namespace DevUp.Infrastructure.Persistence.MongoDb.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserDto> _userCollection;
        private readonly IMapper _mapper;

        public UserRepository(IMongoDatabase identityDatabase, IMapper mapper)
        {
            _userCollection = identityDatabase.GetCollection<UserDto>("users");
            _mapper = mapper;
        }

        public async Task<User> AddAsync(User user)
        {
            var dto = _mapper.Map<UserDto>(user);
            await _userCollection.InsertOneAsync(dto);
            return user;
        }

        public Task DeleteAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            var filter = Builders<UserDto>.Filter.Empty;
            var cursor = await _userCollection.FindAsync(filter);
            var dtos = await cursor.ToListAsync();
            return _mapper.Map<IReadOnlyList<User>>(dtos);
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
