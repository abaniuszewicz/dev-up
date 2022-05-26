using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            CreateMap<RefreshToken, RefreshTokenDto>()
                .ForMember(d => d.Token, opt => opt.MapFrom(s => s.Id.Token))
                .ForMember(d => d.Jti, opt => opt.MapFrom(s => s.Jti))
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.UserId.Id))
                .ForMember(d => d.CreationDate, opt => opt.MapFrom(s => s.Lifespan.Start))
                .ForMember(d => d.ExpiryDate, opt => opt.MapFrom(s => s.Lifespan.End))
                .ForMember(d => d.DeviceId, opt => opt.MapFrom(s => s.DeviceId.Id))
                .ForMember(d => d.Used, opt => opt.MapFrom(s => s.Used))
                .ForMember(d => d.Invalidated, opt => opt.MapFrom(s => s.Invalidated));

            CreateMap<UserDto, User>().ConvertUsing(d => new User(new(d.Id), new(d.Username)));
            CreateMap<RefreshTokenDto, RefreshToken>()
                .ConvertUsing(d => new RefreshToken(new(d.Token), d.Jti, new(d.UserId), new(d.DeviceId), new(d.CreationDate, d.ExpiryDate)));
            CreateMap<UserDto, PasswordHash>().ForMember(d => d.Value, opt => opt.MapFrom(s => s.PasswordHash));
        }
    }
}
