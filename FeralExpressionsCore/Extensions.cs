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
            var visitor = new ExpressionInliningVisitor();
            return queryable.WithExpressionVisitors(visitor);
        }

        public static Expression MapTypes(this Expression source, IDictionary<Type,Type> mappings)
        {
            var mapper = new OfTypeMappingVisitor(mappings);

            return mapper.Visit(source);
        }

        public static IQueryable<T> MapTypes<T>(this IQueryable<T> queryable, IDictionary<Type, Type> mappings)
        {
            var visitor = new OfTypeMappingVisitor(mappings);
            return queryable.WithExpressionVisitors(visitor);
        }

        public static IQueryable<T> WithExpressionVisitors<T>(this IQueryable<T> queryable, params ExpressionVisitor[] visitors)
        {
            IQueryProvider provider;
            if (queryable.Provider is VisitingQueryProvider visitingQueryProvider)
            {
                foreach (var visitor in visitors)
                {
                    visitingQueryProvider.ExpressionVisitors.Add(visitor);
                }
                provider = visitingQueryProvider;
            }
            else
                provider = new VisitingQueryProvider(queryable.Provider, visitors);

            return provider.CreateQuery<T>(queryable.Expression);
        }

    }
}
