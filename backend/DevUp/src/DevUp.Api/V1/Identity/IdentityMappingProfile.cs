using System;
using System.Linq;
using AutoMapper;
using DevUp.Api.Contracts.V1.Identity.Requests;
using DevUp.Api.Contracts.V1.Identity.Responses;
using DevUp.Application.Identity.Commands;
using DevUp.Domain.Identity;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Api.V1.Identity
{
    internal class IdentityMappingProfile : Profile
    {
        public IdentityMappingProfile()
        {
            CreateMap<RegisterUserRequest, Username>().ConvertUsing(s => new Username(s.Username));
            CreateMap<RegisterUserRequest, Password>().ConvertUsing(s => new Password(s.Password));

            CreateMap<LoginUserRequest, Username>().ConvertUsing(s => new Username(s.Username));
            CreateMap<LoginUserRequest, Password>().ConvertUsing(s => new Password(s.Password));

            CreateMap<RefreshUserRequest, Token>().ConvertUsing(s => new Token(s.Token));
            CreateMap<RefreshUserRequest, RefreshToken>().ConvertUsing(s => new RefreshToken(s.RefreshToken));

            CreateMap<DeviceRequest, Device>().ConvertUsing(s => new Device(new(s.Id), s.Name));

            CreateMap<IdentityResult, IdentityResponse>().ConvertUsing(s => new() { Success = true, Errors = Array.Empty<string>(), Token = s.Token.Value, RefreshToken = s.RefreshToken.Value });
            CreateMap<DomainException, IdentityResponse>().ConvertUsing(s => new() { Success = false, Errors = s.Errors.ToArray(), Token = null, RefreshToken = null });

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
        }
    }
}
