using AutoMapper;
using DevUp.Application.Organization.Queries.Results;
using DevUp.Domain.Organization.Entities;

namespace DevUp.Application.Organization
{
    internal class OrganizationMappingProfile : Profile
    {
        public OrganizationMappingProfile()
        {
            CreateMap<Team, TeamQueryResult>()
                .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id.Id))
                .ForMember(d => d.Name, opts => opts.MapFrom(s => s.Name.Value));
        }
    }
}
