using System;
using System.Linq;
using AutoMapper;
using DevUp.Api.Contracts.V1.Identity.Requests;
using DevUp.Api.Contracts.V1.Identity.Responses;
using DevUp.Domain.Identity;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.ValueObjects;

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
            CreateMap<IdentityException, IdentityResponse>().ConvertUsing(s => new() { Success = false, Errors = s.Errors.ToArray(), Token = null, RefreshToken = null });
        }
    }
}
