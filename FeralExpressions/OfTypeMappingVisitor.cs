using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FeralExpressions
{
    public class OfTypeMappingVisitor : ExpressionVisitor
    {
        public OfTypeMappingVisitor(IDictionary<Type,Type> mappings)
        {
            this.mappings = mappings;

            var enumerableType = typeof(Enumerable);
            method_OfType = enumerableType.GetMethod("OfType", BindingFlags.Static | BindingFlags.Public);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if(IsMethod_OfType(node))
            {
                node = MapTypeInMethod(node);
            }
                return base.VisitMethodCall(node);

        }

        private MethodCallExpression MapTypeInMethod(MethodCallExpression node)
        {
            var genericSourceArg = node.Method.GetGenericArguments()[0];

            if (mappings.ContainsKey(genericSourceArg))
            {
                var genericDestArg = mappings[genericSourceArg];

                var genericMethod = node.Method.GetGenericMethodDefinition();
                var destMethod = genericMethod.MakeGenericMethod(genericDestArg);

                return Expression.Call(node.Object, destMethod, node.Arguments);
            }
            else
                return node;
        }

        private bool IsMethod_OfType(MethodCallExpression node)
        {
            return node.Method.IsGenericMethod && node.Method.GetGenericMethodDefinition() == method_OfType;

        }

        private readonly MethodInfo method_OfType;
        private readonly IDictionary<Type, Type> mappings;
    }
}
