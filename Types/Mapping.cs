﻿using MorphMapper.Interfaces;
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

        public Mapping<TSource, TDestination> ForMember(Expression<Func<TDestination, object>> destination, Func<MappingOption<TSource>, Expression> source)
        {
            var destinationProperty = GetPropertyInfo(destination);

            // todo:Validate types on register
            //if (destinationProperty.PropertyType != source.GetType())
            //{
            //    throw new InvalidOperationException($"Type mismatch while mapping property - {destinationProperty.Name}");
            //}

            propertyMappings.Add(destinationProperty, source(new MappingOption<TSource>()));

            return this;
        }

        public Mapping<TSource, TDestination> ReverseMap()
        {
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
