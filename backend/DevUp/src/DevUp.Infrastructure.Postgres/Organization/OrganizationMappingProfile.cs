using AutoMapper;
using DevUp.Application.Organization.Queries.Results;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.ValueObjects;
using DevUp.Infrastructure.Postgres.Organization.Dtos;

namespace DevUp.Infrastructure.Postgres.Organization
{
    internal class OrganizationMappingProfile : Profile
    {
        public OrganizationMappingProfile()
        {
            CreateMap<Team, TeamDto>()
                .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id.Id))
                .ForMember(d => d.Name, opts => opts.MapFrom(s => s.Name.Value));

            CreateMap<TeamDto, TeamQueryResult>()
                .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
                .ForMember(d => d.Name, opts => opts.MapFrom(s => s.Name));

            CreateMap<TeamDto, Team>().ConvertUsing(s => new Team(new TeamId(s.Id), new TeamName(s.Name)));
        }
    }
}
