using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions
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
            return new InlineWrapper<T>(queryable);
        }

        private class InlineWrapper<T> : IQueryable<T>
        {
            public InlineWrapper(IQueryable<T> inner)
            {
                this.inner = inner;
            }

            public Expression Expression => inner.Expression.Inline();

            public Type ElementType => typeof(T);

            public IQueryProvider Provider => throw new NotImplementedException();

            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            private class QueryProvider : IQueryProvider
            {
                public IQueryable CreateQuery(Expression expression)
                {
                    throw new NotImplementedException();
                }

                public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
                {
                    throw new NotImplementedException();
                }

                public object Execute(Expression expression)
                {
                    throw new NotImplementedException();
                }

                public TResult Execute<TResult>(Expression expression)
                {
                    throw new NotImplementedException();
                }
            }

            private IQueryable<T> inner;

        }
    }
}
