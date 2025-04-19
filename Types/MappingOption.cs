using System.Linq.Expressions;

namespace MorphMapper.Types
{
    public class MappingOption<Property>
    {
        public Expression<Func<Property, object>> MapFrom(Expression<Func<Property, object>> expression)
        {
            return expression;
        }

        public Expression<Func<object, object>> Ignore()
        {
            return (ignore) => ignore;
        }
    }
}
