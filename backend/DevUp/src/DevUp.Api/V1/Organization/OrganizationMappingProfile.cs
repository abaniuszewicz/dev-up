using AutoMapper;
using DevUp.Api.Contracts.V1.Organization.Requests;
using DevUp.Application.Organization.Commands;

namespace DevUp.Api.V1.Organization
{
    public class OrganizationMappingProfile : Profile
    {
        public OrganizationMappingProfile()
        {
            CreateMap<CreateTeamRequest, CreateTeamCommand>()
                .ForMember(c => c.Name, opts => opts.MapFrom(r => r.Name));
        }
    }
}
