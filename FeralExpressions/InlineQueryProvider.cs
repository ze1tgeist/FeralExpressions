using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions
{
    class InlineQueryProvider<T> : IQueryProvider
    {
        public InlineQueryProvider(InlineQueryable<T> queryable)
        {
            this.queryable = queryable;
        }
        public IQueryable CreateQuery(Expression expression)
        {
            return queryable.Inner.Provider.CreateQuery(expression.Inline());
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return queryable.Inner.Provider.CreateQuery<TElement>(expression.Inline());
        }

        public object Execute(Expression expression)
        {
            return queryable.Inner.Provider.Execute(expression.Inline());
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return queryable.Inner.Provider.Execute<TResult>(expression.Inline());
        }

        private InlineQueryable<T> queryable;
    }
}
