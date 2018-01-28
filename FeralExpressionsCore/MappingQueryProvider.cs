using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FeralExpressionsCore
{
    class MappingQueryProvider<T> : EntityQueryProvider
    {
        public MappingQueryProvider(MappingQueryable<T> queryable, IDictionary<Type, Type> mappings) : base(new DummyQueryCompiler())
        {
            this.queryable = queryable;
            this.mappings = mappings;
        }

        public override IQueryable CreateQuery(Expression expression)
        {
            return queryable.Inner.Provider.CreateQuery(expression.MapTypes(mappings));
        }

        public override IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new MappingQueryable<TElement>(queryable.Inner.Provider.CreateQuery<TElement>(expression.MapTypes(mappings)), mappings);
        }

        public override object Execute(Expression expression)
        {
            return queryable.Inner.Provider.Execute(expression.MapTypes(mappings));
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            return queryable.Inner.Provider.Execute<TResult>(expression.MapTypes(mappings));
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

        private MappingQueryable<T> queryable;
        private IDictionary<Type, Type> mappings;
    }
}
