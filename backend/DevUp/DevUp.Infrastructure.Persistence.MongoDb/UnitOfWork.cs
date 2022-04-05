using AutoMapper;
using DevUp.Infrastructure.Persistence.MongoDb.Repositories;
using DevUp.Infrastructure.Persistence.Repositories;
using MongoDB.Driver;

namespace DevUp.Infrastructure.Persistence.MongoDb
{
    internal class UnitOfWork : IUnitOfWork, IAsyncDisposable
    {
        private readonly IClientSessionHandle _session;

        public IUserRepository UserRepository { get; }

        public UnitOfWork(IMongoClient mongoClient, IMapper mapper)
        {
            UserRepository = new UserRepository(mongoClient.GetDatabase("identity"), mapper);

            _session = mongoClient.StartSession();
            _session.StartTransaction();
        }

        public async Task SaveChangesAsync()
        {
            await _session.CommitTransactionAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _session.AbortTransactionAsync();
        }
    }
}
