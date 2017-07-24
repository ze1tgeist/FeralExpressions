using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions
{
    public static class Extensions
    {
        public static Expression<T> Inline<T>(this Expression<T> source)
        {
            var inliner = new ExpressionInliningVisitor();

            return (Expression<T>) inliner.Visit(source);
        }
    }
}
