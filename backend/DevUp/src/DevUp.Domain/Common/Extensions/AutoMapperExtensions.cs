using AutoMapper;

namespace DevUp.Domain.Common.Extensions
{
    public static class AutoMapperExtensions
    {
        public static TDestination MapOrNull<TDestination>(this IMapper mapper, object source)
            where TDestination : class
        {
            return source is null ? null : mapper.Map<TDestination>(source);
        }
    }
}
