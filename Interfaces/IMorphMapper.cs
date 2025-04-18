using MorphMapper.Types;

namespace MorphMapper.Interfaces
{
    internal interface IMorphMapper
    {
        Mapping<TSource, TDestination> CreateMap<TSource, TDestination>();
        TDestination Map<TSource, TDestination>(TSource source) where TDestination : new();
    }
}
