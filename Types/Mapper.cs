using MorphMapper.Interfaces;
using MorphMapperLibrary.Types;
using System.Linq.Expressions;

namespace MorphMapper.Types
{
    public class Mapper(MapperConfiguration configuration)
    {
        private readonly List<IMapping> mappings = configuration.GetMappings();
        public List<IMapping> GetMappings() => mappings;

        /// <summary>
        /// Maps properties from a source object of type <typeparamref name="TSource"/> 
        /// to a new instance of type <typeparamref name="TDestination"/>.
        /// The mapping is performed based on the property mappings defined in the 
        /// <see cref="Mapping{TSource, TDestination}"/> configuration.
        /// </summary>
        /// <typeparam name="TSource">The type of the source object.</typeparam>
        /// <typeparam name="TDestination">The type of the destination object.</typeparam>
        /// <param name="source">The source object to map from.</param>
        /// <returns>
        /// A new instance of <typeparamref name="TDestination"/> with properties 
        /// mapped from the source object.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no mapping configuration exists for the specified source and 
        /// destination types, or
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