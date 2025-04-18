using System.Linq.Expressions;

namespace MorphMapper.Types
{
    public class MappingOption
    {
        public void MapFrom(Expression<Func<object, object>> expression)
        {

        }

        public void Ignore(Expression<Func<object, object>> expression)
        {

        }
    }
}
