using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FeralExpressions
{
    class MappingQueryable<T> : IQueryable<T>
    {
        public MappingQueryable(IQueryable<T> inner, IDictionary<Type, Type> mappings)
        {
            this.inner = inner;
            this.mappings = mappings;
            provider = new MappingQueryProvider<T>(this, mappings);
        }

        public Expression Expression => inner.Expression.MapTypes(mappings);

        public Type ElementType => typeof(T);

        public IQueryProvider Provider => provider;

        public IEnumerator<T> GetEnumerator() => inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();

        internal IQueryable<T> Inner => inner;

        private readonly IQueryable<T> inner;
        private readonly IDictionary<Type, Type> mappings;
        private IQueryProvider provider;

    }
}
