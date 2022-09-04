using AutoMapper;
using DevUp.Api.Contracts.V1.Identity.Requests;
using DevUp.Application.Identity.Commands;

namespace DevUp.Api.V1.Identity
{
    internal class IdentityMappingProfile : Profile
    {
        public IdentityMappingProfile()
        {
            CreateMap<RegisterUserRequest, RegisterUserCommand>()
                .ForMember(c => c.Username, m => m.MapFrom(r => r.Username))
                .ForMember(c => c.Password, m => m.MapFrom(r => r.Password))
                .ForMember(c => c.DeviceId, m => m.MapFrom(r => r.Device.Id))
                .ForMember(c => c.DeviceName, m => m.MapFrom(r => r.Device.Name));

            CreateMap<LoginUserRequest, LoginUserCommand>()
                .ForMember(c => c.Username, m => m.MapFrom(r => r.Username))
                .ForMember(c => c.Password, m => m.MapFrom(r => r.Password))
                .ForMember(c => c.DeviceId, m => m.MapFrom(r => r.Device.Id))
                .ForMember(c => c.DeviceName, m => m.MapFrom(r => r.Device.Name));

            CreateMap<RefreshUserRequest, RefreshUserCommand>()
                .ForMember(c => c.Token, m => m.MapFrom(r => r.Token))
                .ForMember(c => c.RefreshToken, m => m.MapFrom(r => r.RefreshToken))
                .ForMember(c => c.DeviceId, m => m.MapFrom(r => r.Device.Id))
                .ForMember(c => c.DeviceName, m => m.MapFrom(r => r.Device.Name));
        }
    }
}
