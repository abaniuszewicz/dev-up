using AutoMapper;

namespace DevUp.Application.Identity
{
    internal class IdentityMappingProfile : Profile
    {
        public IdentityMappingProfile()
        {
            CreateMap<TokenPair, TokenPair>()
                .ForMember(tp => tp.Token, m => m.MapFrom(ir => ir.Token.Value))
                .ForMember(tp => tp.RefreshToken, m => m.MapFrom(ir => ir.RefreshToken.Value));
        }
    }
}
