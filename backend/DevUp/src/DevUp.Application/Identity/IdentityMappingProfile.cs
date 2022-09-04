using AutoMapper;
using DevUp.Domain.Identity;

namespace DevUp.Application.Identity
{
    internal class IdentityMappingProfile : Profile
    {
        public IdentityMappingProfile()
        {
            CreateMap<IdentityResult, TokenPair>()
                .ForMember(tp => tp.Token, m => m.MapFrom(ir => ir.Token.Value))
                .ForMember(tp => tp.RefreshToken, m => m.MapFrom(ir => ir.RefreshToken.Value));
        }
    }
}
