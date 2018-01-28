using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressionsCore
{
    public static class Extensions
    {
        public static Expression Inline(this Expression source)
        {
            var inliner = new ExpressionInliningVisitor();

            return inliner.Visit(source);
        }

        public static IQueryable<T> Inline<T>(this IQueryable<T> queryable)
        {
            return new InlineQueryable<T>(queryable);
        }

        public static Expression MapTypes(this Expression source, IDictionary<Type,Type> mappings)
        {
            var mapper = new OfTypeMappingVisitor(mappings);

            return mapper.Visit(source);
        }

        public static IQueryable<T> MapTypes<T>(this IQueryable<T> queryable, IDictionary<Type, Type> mappings)
        {
            return new MappingQueryable<T>(queryable, mappings);
        }

    }
}
