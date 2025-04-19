using MorphMapper.Interfaces;
using MorphMapper.Types;

namespace MorphMapperLibrary.Types
{
    public class MapperConfiguration
    {
        private readonly List<IMapping> mappings = [];

        public List<IMapping> GetMappings() => mappings;
        public Mapper CreateMapper() => new(this);

        public Mapping<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            Mapping<TSource, TDestination> mapping = new Mapping<TSource, TDestination>();

            mappings.Add(mapping);

            return mapping;
        }
    }
}
