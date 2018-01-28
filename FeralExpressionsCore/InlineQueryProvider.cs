using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using System.Threading;

namespace FeralExpressionsCore
{
    class InlineQueryProvider<T> : EntityQueryProvider, IQueryProvider
    {
        public InlineQueryProvider(InlineQueryable<T> queryable) : base(new DummyQueryCompiler())
        {
            this.queryable = queryable;
        }
        public override IQueryable CreateQuery(Expression expression)
        {
            return queryable.Inner.Provider.CreateQuery(expression.Inline());
        }

        public override IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new InlineQueryable<TElement>(queryable.Inner.Provider.CreateQuery<TElement>(expression.Inline()));
        }

        public override object Execute(Expression expression)
        {
            return queryable.Inner.Provider.Execute(expression.Inline());
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            return queryable.Inner.Provider.Execute<TResult>(expression.Inline());
        }

        public override IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public override Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private class DummyQueryCompiler : IQueryCompiler
        {
            public Func<QueryContext, IAsyncEnumerable<TResult>> CreateCompiledAsyncEnumerableQuery<TResult>(Expression query)
            {
                throw new NotImplementedException();
            }

            public Func<QueryContext, Task<TResult>> CreateCompiledAsyncTaskQuery<TResult>(Expression query)
            {
                throw new NotImplementedException();
            }

            public Func<QueryContext, TResult> CreateCompiledQuery<TResult>(Expression query)
            {
                throw new NotImplementedException();
            }

            public TResult Execute<TResult>(Expression query)
            {
                throw new NotImplementedException();
            }

            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression query)
            {
                throw new NotImplementedException();
            }

            public Task<TResult> ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        private InlineQueryable<T> queryable;
    }
}
