using MorphMapper.Interfaces;
using MorphMapper.Types;
using System.Reflection;

namespace MorphMapperLibrary.Types
{
    public class MapperConfiguration
    {
        private readonly List<IMapping> mappings = [];

        #region Constructor Overloads
        public MapperConfiguration() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapperConfiguration"/> class using the provided assembly.
        /// This constructor scans the specified assembly for types that inherit from <see cref="Mapping{TSource, TDestination}"/>.
        /// It creates instances of these mapping types, invokes their <c>Configure</c> method, and registers the resulting mappings.
        /// </summary>
        /// <param name="assembly">The assembly to scan for mapping types. If null, no mappings will be registered.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a mapping type does not have a <c>Configure</c> method or if the method fails to return a valid mapping.
        /// </exception>
        public MapperConfiguration(Assembly? assembly = null)
        {
            var existingTypes = assembly?.GetTypes().Where(t => t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(Mapping<,>)) ?? [];

            foreach (var type in existingTypes)
            {
                var typeInstance = Activator.CreateInstance(type) as IMapping;

                if (typeInstance != null)
                {
                    var configureMethod = typeInstance.GetType().GetMethod("Configure");
                    if (configureMethod != null)
                    {
                        var mapping = configureMethod.Invoke(typeInstance, new object[] { typeInstance }) as IMapping;
                        if (mapping != null)
                        {
                            mappings.Add(mapping);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("The Configure method was not found on the mapping type.");
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapperConfiguration"/> class using the provided list of mapping types.
        /// This constructor registers all the mappings by creating instances of the specified mapping types.
        /// Each mapping type must implement the <see cref="IMapping"/> interface.
        /// </summary>
        /// <param name="mappingTypes">A list of types that implement the <see cref="IMapping"/> interface.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a mapping type does not implement the <see cref="IMapping"/> interface
        /// or if an instance of the mapping type cannot be created.
        /// </exception>
        public MapperConfiguration(List<Type> mappingTypes)
        {
            foreach (var mappingType in mappingTypes)
            {
                if (typeof(IMapping).IsAssignableFrom(mappingType))
                {
                    var instance = Activator.CreateInstance(mappingType) as IMapping;
                    if (instance != null)
                    {
                        mappings.Add(instance);
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to create an instance of the mapping type.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Unsupported mapping type provided.");
                }
            }
        }

        /// <summary>
        /// Creates a mapping configuration between a source type <typeparamref name="TSource"/> 
        /// and a destination type <typeparamref name="TDestination"/>.
        /// This method initializes a new instance of <see cref="Mapping{TSource, TDestination}"/>, 
        /// adds it to the internal mappings list, and returns the mapping for further configuration.
        /// </summary>
        /// <typeparam name="TSource">The source type for the mapping.</typeparam>
        /// <typeparam name="TDestination">The destination type for the mapping.</typeparam>
        /// <returns>
        /// A <see cref="Mapping{TSource, TDestination}"/> instance that can be further configured.
        /// </returns>
        public Mapping<TSource, TDestination> CreateMap<TSource, TDestination>()
        {

            Mapping<TSource, TDestination> mapping = new();

            mappings.Add(mapping);

            return mapping;
        }

        #endregion

        /// <summary>
        /// Creates a new instance of the <see cref="Mapper"/> class using the current configuration.
        /// This method initializes a <see cref="Mapper"/> with all the mappings defined in this configuration.
        /// </summary>
        /// <returns>A new <see cref="Mapper"/> instance configured with the current mappings.</returns>
        public Mapper CreateMapper() => new(this);

        /// <summary>
        /// Retrieves all the mappings defined in the current configuration.
        /// </summary>
        /// <returns>A list of mappings that implement the <see cref="IMapping"/> interface.</returns>
        public List<IMapping> GetMappings() => mappings;
    }
}
