using System;
using AutoMapper;
using DevUp.Api.Contracts.V1.Organization.Requests;
using DevUp.Api.Contracts.V1.Organization.Responses;
using DevUp.Application.Organization.Commands;
using DevUp.Application.Organization.Queries.Results;

namespace DevUp.Api.V1.Organization
{
    internal class OrganizationMappingProfile : Profile
    {
        public OrganizationMappingProfile()
        {
            CreateMap<CreateTeamRequest, CreateTeamCommand>()
                .ForMember(d => d.Name, opts => opts.MapFrom(s => s.Name));

            CreateMap<TeamQueryResult, TeamResponse>()
                .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
                .ForMember(d => d.Name, opts => opts.MapFrom(s => s.Name))
                .ForMember(d => d.Members, opts => opts.MapFrom(s => Array.Empty<MemberResponse>()));
        }
    }
}
