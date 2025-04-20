using System.Linq.Expressions;

namespace MorphMapper.Types
{
    public class MappingOption<Property>
    {
        /// <summary>
        /// Specifies the source property from which the target property will be mapped.
        /// </summary>
        /// <param name="expression">An expression that selects the source property.</param>
        /// <returns>The provided expression for further configuration.</returns>
        public Expression<Func<Property, object>> MapFrom(Expression<Func<Property, object>> expression)
        {
            return expression;
        }

        /// <summary>
        /// Ignores the mapping of the current property.
        /// </summary>
        /// <returns>An expression that represents the ignored property.</returns>
        public Expression<Func<object, object>> Ignore()
        {
            return (ignore) => ignore;
        }
    }
}
