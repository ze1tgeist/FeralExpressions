using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FeralExpressionsCore
{
    public class OfTypeMappingVisitor : ExpressionVisitor
    {
        public OfTypeMappingVisitor(IDictionary<Type,Type> mappings)
        {
            this.mappings = mappings;

            var enumerableType = typeof(Enumerable);
            method_enumerable_OfType = enumerableType.GetMethod("OfType", BindingFlags.Static | BindingFlags.Public);

            var queryableType = typeof(Queryable);
            method_queryable_OfType = queryableType.GetMethod("OfType", BindingFlags.Static | BindingFlags.Public);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if(IsMethod_OfType(node))
            {
                node = MapTypeInMethod(node);
            }

            return base.VisitMethodCall(node);

        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            if (node.NodeType == ExpressionType.TypeIs && mappings.ContainsKey(node.TypeOperand))
            {
                node = Expression.TypeIs(node.Expression, mappings[node.TypeOperand]);
            }

            return base.VisitTypeBinary(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Convert && mappings.ContainsKey(node.Type))
            {
                node = Expression.Convert(node.Operand, mappings[node.Type]);
            }
            else if (node.NodeType == ExpressionType.TypeAs && mappings.ContainsKey(node.Type))
            {
                node = Expression.TypeAs(node.Operand, mappings[node.Type]);
            }
            return base.VisitUnary(node);
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
            return 
                node.Method.IsGenericMethod 
                && 
                    (
                        node.Method.GetGenericMethodDefinition() == method_enumerable_OfType
                        || node.Method.GetGenericMethodDefinition() == method_queryable_OfType
                    );

        }

        private readonly MethodInfo method_enumerable_OfType;
        private readonly MethodInfo method_queryable_OfType;
        private readonly IDictionary<Type, Type> mappings;
    }
}
