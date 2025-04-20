using MorphMapper.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace MorphMapper.Types
{
    public class Mapping<TSource, TDestination> : IMapping
    {
        private readonly int hash = typeof(TSource).GetHashCode() + typeof(TDestination).GetHashCode();
        public int Hash => hash;

        private bool isReversed = false;
        public bool IsReversed => isReversed;

        public Dictionary<PropertyInfo, Expression> propertyMappings = new();

        public virtual Mapping<TSource, TDestination> Configure(Mapping<TSource, TDestination> mapping)
        {
            return this;
        }

        /// <summary>
        /// Configures the mapping for a specific member of the destination type.
        /// This method allows you to define how a property in the destination type
        /// should be mapped from the source type by providing a mapping option.
        /// </summary>
        /// <param name="destination">
        /// An expression representing the property of the destination type to be mapped.
        /// </param>
        /// <param name="source">
        /// A function that takes a <see cref="MappingOption{TSource}"/> and returns an expression
        /// defining how the source property maps to the destination property.
        /// </param>
        /// <returns>
        /// The current <see cref="Mapping{TSource, TDestination}"/> instance to allow method chaining.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the provided destination expression is not a valid property access expression.
        /// </exception>
        public Mapping<TSource, TDestination> ForMember(Expression<Func<TDestination, object>> destination, Func<MappingOption<TSource>, Expression> source)
        {
            var destinationProperty = GetPropertyInfo(destination);

            // todo:Validate types (e.g. int to int, s on register
            //if (destinationProperty.PropertyType != source.GetType())
            //{
            //    throw new InvalidOperationException($"Type mismatch while mapping property - {destinationProperty.Name}");
            //}

            propertyMappings.Add(destinationProperty, source(new MappingOption<TSource>()));

            return this;
        }

        public Mapping<TSource, TDestination> ReverseMap()
        {
            //todo: add this feature
            this.isReversed = true;

            return this;
        }

        internal PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return (PropertyInfo)memberExpression.Member;
            }
            else if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression operand)
            {
                return (PropertyInfo)operand.Member;
            }

            throw new ArgumentException("Expression must be a property access.");
        }
    }
}
