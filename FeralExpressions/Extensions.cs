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

    }
}
