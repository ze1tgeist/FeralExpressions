using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions
{
    class ExpressionInliningVisitor : ExpressionVisitor
    {

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var expr = GetExpressionEquivalent(node.Method);

            return base.VisitMethodCall(node);
        }

        private LambdaExpression GetExpressionEquivalent(MethodInfo method)
        {
            var bindingFlags = BindingFlags.Static
                | (method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic);

            var property = method.DeclaringType.GetProperty(method.Name + "_Expression", bindingFlags);
            if (property != null && FunctionMatchesExpressionType(method, property.PropertyType))
            {
                var expression = property.GetValue(null);
                return expression as LambdaExpression;
            }
            else
            {
                return null;
            }
        }

        private Expression InlineExpression(LambdaExpression lambda, MethodCallExpression method)
        {
            return null;
        }

        private bool FunctionMatchesExpressionType(MethodInfo method, Type type)
        {
            return
                (type.GetGenericTypeDefinition() == typeof(System.Linq.Expressions.Expression<>))
                && type.GetGenericArguments().FirstOrDefault()?.GetGenericTypeDefinition()?.FullName == $"System.Func`{method.GetParameters().Count() + (method.IsStatic ? 1 : 2)}"
                && (type.GetGenericArguments().FirstOrDefault()?.GetGenericArguments()
                    .Skip(method.IsStatic ? 0 : 1)
                    .Take(method.GetParameters().Count())
                    .Zip(method.GetParameters().Select(p => p.ParameterType), (funcArg, methArg) => funcArg == methArg)
                    .All(b => b) ?? false)
                && (method.IsStatic || type.GetGenericArguments().FirstOrDefault()?.GetGenericArguments().First() == method.DeclaringType)
                && type.GetGenericArguments().FirstOrDefault()?.GetGenericArguments().Last() == method.ReturnType;

        }
    }
}
