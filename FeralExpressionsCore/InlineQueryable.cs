using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressionsCore
{
    class InlineQueryable<T> : IQueryable<T>
    {
        public InlineQueryable(IQueryable<T> inner)
        {
            this.inner = inner;
            this.provider = new InlineQueryProvider<T>(this);
        }

        public Expression Expression => inner.Expression.Inline();

        public Type ElementType => typeof(T);

        public IQueryProvider Provider => provider;

        public IEnumerator<T> GetEnumerator() => 
            inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();

        internal IQueryable<T> Inner => inner;

        private IQueryable<T> inner;
        private IQueryProvider provider;

    }
}
