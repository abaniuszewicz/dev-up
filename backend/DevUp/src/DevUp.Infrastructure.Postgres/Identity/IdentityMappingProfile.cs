using AutoMapper;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;
using DevUp.Infrastructure.Postgres.Identity.Dtos;

namespace DevUp.Infrastructure.Postgres.Identity
{
    internal class IdentityMappingProfile : Profile
    {
        public IdentityMappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Id))
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.Username.Value))
                .ForMember(d => d.PasswordHash, opt => opt.Ignore());
            CreateMap<RefreshTokenInfo, RefreshTokenDto>()
                .ForMember(d => d.Token, opt => opt.MapFrom(s => s.Id.Value))
                .ForMember(d => d.Jti, opt => opt.MapFrom(s => s.Jti))
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.UserId.Id))
                .ForMember(d => d.CreationDate, opt => opt.MapFrom(s => s.Lifespan.Start))
                .ForMember(d => d.ExpiryDate, opt => opt.MapFrom(s => s.Lifespan.End))
                .ForMember(d => d.DeviceId, opt => opt.MapFrom(s => s.DeviceId.Id))
                .ForMember(d => d.Used, opt => opt.MapFrom(s => s.Used))
                .ForMember(d => d.Invalidated, opt => opt.MapFrom(s => s.Invalidated));
            CreateMap<Device, DeviceDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name));

            CreateMap<UserDto, User>().ConvertUsing(s => new User(new(s.Id), new(s.Username)));
            CreateMap<RefreshTokenDto, RefreshTokenInfo>().ConvertUsing<RefreshTokenMapper>();
            CreateMap<UserDto, PasswordHash>().ConvertUsing(s => new PasswordHash(s.PasswordHash));
            CreateMap<DeviceDto, Device>().ConvertUsing(s => new Device(new(s.Id), new(s.Name)));
        }

        private class RefreshTokenMapper : ITypeConverter<RefreshTokenDto, RefreshTokenInfo>
        {
            public RefreshTokenInfo Convert(RefreshTokenDto dto, RefreshTokenInfo refreshToken, ResolutionContext context)
            {
                return new RefreshTokenInfo(new(dto.Token), dto.Jti, new(dto.UserId), new(dto.DeviceId), new(dto.CreationDate, dto.ExpiryDate))
                {
                    Used = dto.Used,
                    Invalidated = dto.Invalidated
                };
            }
        }
    }
}
