using MorphMapper.Interfaces;

namespace MorphMapper.Types
{
    public class MorphMapper
    {
        private List<IMapping> mappings = [];

        public Mapping<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            Mapping<TSource, TDestination> mapping = new Mapping<TSource, TDestination>();

            mappings.Add(mapping);

            return mapping;
        }

        public TDestination Map<TSource, TDestination>(TSource source) where TDestination : new()
        {
            var currentTypesHash = typeof(TSource).GetHashCode() + typeof(TDestination).GetHashCode();

            var mappingBase = mappings.Find(x => x.Hash == currentTypesHash);
            Mapping<TSource, TDestination>? mapping = null;

            if (mappingBase is not null)
            {
                mapping = mappingBase as Mapping<TSource, TDestination>;
            }

            if(mappingBase is null || mapping is null)
            {
                throw new InvalidOperationException($"No mapping found for types {typeof(TSource).Name} and {typeof(TDestination).Name}.");
            }

            var sourceProperties = typeof(TSource).GetProperties().ToList();
            var destinationProperties = typeof(TDestination).GetProperties().ToList();

            TDestination newObject = new();

            foreach(var property in destinationProperties)
            {
                if(mapping.propertyMappings.ContainsKey(property))
                {
                    var sourceProperty = sourceProperties.Find(x => x == mapping.propertyMappings[property]);

                    if (sourceProperty is not null)
                    {
                        property.SetValue(newObject, sourceProperty.GetValue(source));
                    }
                }
                else
                {
                    var sourceProperty = sourceProperties.Find(x => x.Name == property.Name);

                    if (sourceProperty is not null)
                    {
                        property.SetValue(newObject, sourceProperty.GetValue(source));
                    }
                }
            }

            return newObject;
        }
    }
}