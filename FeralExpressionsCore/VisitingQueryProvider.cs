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
    public class VisitingQueryProvider : EntityQueryProvider
    {
        private readonly IAsyncQueryProvider inner;
        public IList<ExpressionVisitor> ExpressionVisitors { get; }

        public VisitingQueryProvider(IQueryProvider inner, IEnumerable<ExpressionVisitor> expressionVisitors) : base(GetQueryCompiler(inner))
        {
            this.inner = inner as IAsyncQueryProvider;
            this.ExpressionVisitors = new List<ExpressionVisitor>(expressionVisitors);
        }

        public override object Execute(Expression expression)
        {
            var expr = VisitExpression(expression);
            return base.Execute(expr);
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            var expr = VisitExpression(expression);
            return base.Execute<TResult>(expr);
        }

        public override TResult ExecuteAsync<TResult>(Expression expression)
        {
            var expr = VisitExpression(expression);
            return base.ExecuteAsync<TResult>(expr);
        }

        public override Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var expr = VisitExpression(expression);
            return base.ExecuteAsync<TResult>(expr, cancellationToken);
        }

        private Expression VisitExpression(Expression expression)
        {
            var expr = expression;
            foreach(var visitor in ExpressionVisitors)
            {
                expr = visitor.Visit(expr);
            }

            return expr;
        }

        private static IQueryCompiler GetQueryCompiler(IQueryProvider inner)
        {
            if (inner is EntityQueryProvider entityQueryProvider)
            {
                var queryCompilerField = typeof(EntityQueryProvider).GetField("_queryCompiler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var compiler = queryCompilerField.GetValue(entityQueryProvider) as IQueryCompiler;

                return compiler;
            }
            else
                return new DummyQueryCompiler();
        }

        private class DummyQueryCompiler : IQueryCompiler
        {
            public Func<QueryContext, IAsyncEnumerable<TResult>> CreateCompiledAsyncEnumerableQuery<TResult>(Expression query)
            {
                throw new NotImplementedException();
            }

            public Func<QueryContext, TResult> CreateCompiledAsyncQuery<TResult>(Expression query)
            {
                throw new NotImplementedException();
            }

            public Func<QueryContext, Task<TResult>> CreateCompiledAsyncSingletonQuery<TResult>(Expression query)
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

            public TResult ExecuteAsync<TResult>(Expression query)
            {
                throw new NotImplementedException();
            }

            public Task<TResult> ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

    }
}
