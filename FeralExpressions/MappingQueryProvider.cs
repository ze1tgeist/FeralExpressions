using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FeralExpressions
{
    class MappingQueryProvider<T> : IQueryProvider
    {
        public MappingQueryProvider(MappingQueryable<T> queryable, IDictionary<Type, Type> mappings)
        {
            this.queryable = queryable;
            this.mappings = mappings;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return queryable.Inner.Provider.CreateQuery(expression.MapTypes(mappings));
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new MappingQueryable<TElement>(queryable.Inner.Provider.CreateQuery<TElement>(expression.MapTypes(mappings)), mappings);
        }

        public object Execute(Expression expression)
        {
            return queryable.Inner.Provider.Execute(expression.MapTypes(mappings));
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return queryable.Inner.Provider.Execute<TResult>(expression.MapTypes(mappings));
        }


        private MappingQueryable<T> queryable;
        private IDictionary<Type, Type> mappings;
    }
}
