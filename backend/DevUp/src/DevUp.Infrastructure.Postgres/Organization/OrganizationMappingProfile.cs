using AutoMapper;
using DevUp.Domain.Organization.Entities;
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
        }
    }
}
