using MorphMapper.Interfaces;
using MorphMapperLibrary.Types;
using System.Linq.Expressions;

namespace MorphMapper.Types
{
    public class Mapper
    {
        private List<IMapping> mappings = [];
        public List<IMapping> GetMappings() => mappings;

        public Mapper(MapperConfiguration configuration)
        {
            this.mappings = configuration.GetMappings();
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
                    mapping.propertyMappings.TryGetValue(property, out var expression);

                    var compiledExpression = expression as LambdaExpression;

                    // If Mapping option ignore is set
                    var isIgnored = compiledExpression.Parameters.Any(x => x.Name == "ignore");

                    if (compiledExpression != null)
                    {
                        var compiledDelegate = compiledExpression.Compile();

                        object? value = null;

                        if(isIgnored)
                        {
                            // todo: figure out what to do with ignored properties
                            //value = compiledDelegate.DynamicInvoke(source.GetType().GetProperty(property.Name).GetValue(source));
                        }
                        else
                        {
                            value = compiledDelegate.DynamicInvoke(source);
                        }

                        property.SetValue(newObject, value);
                    }
                    else
                    {
                        throw new InvalidOperationException("The expression could not be compiled.");
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