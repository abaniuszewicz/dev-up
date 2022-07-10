using System.Data;
using AutoMapper;
using Dapper;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;
using DevUp.Infrastructure.Postgres.Identity.Dtos;
using DevUp.Infrastructure.Postgres.Setup;

namespace DevUp.Infrastructure.Postgres.Identity.Repositories
{
    internal class DeviceRepository : IDeviceRepository
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;

        public DeviceRepository(IDbConnectionFactory connectionFactory, IMapper mapper)
        {
            _connection = connectionFactory?.Create(DbConnectionName.Identity) ?? throw new ArgumentNullException(nameof(connectionFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Device?> AddAsync(Device device, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (device is null)
                throw new ArgumentNullException(nameof(device));

            var dto = _mapper.Map<DeviceDto>(device);
            var sql = @$"INSERT INTO devices (
                            id,
                            name
                        )
                        VALUES (
                            @{nameof(DeviceDto.Id)}, 
                            @{nameof(DeviceDto.Name)}
                        )
                        ON CONFLICT (id)
                        DO UPDATE SET name = @{nameof(DeviceDto.Name)}";

            var affectedRows = await _connection.ExecuteAsync(sql, dto);
            return affectedRows == 0 ? null : device;
        }

        public Task DeleteAsync(Device device, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Device>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Device> GetByIdAsync(DeviceId id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Device> UpdateAsync(Device device, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
