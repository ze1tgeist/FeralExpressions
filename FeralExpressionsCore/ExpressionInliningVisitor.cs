using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressionsCore
{
    class ExpressionInliningVisitor : ExpressionVisitor
    {

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            node = RealizeInterfaceConstant(node);
            var expr = GetExpressionEquivalent(node.Method);
            if (expr != null)
            {
                return InlineExpression(expr, node);
            }
            else
            {
                return base.VisitMethodCall(node);
            }
        }

        private MethodCallExpression RealizeInterfaceConstant(MethodCallExpression node)
        {
            var obj = node.Object;
            var method = node.Method;
            var args = node.Arguments;
            MethodCallExpression realizedNode = null;
            if (method.DeclaringType.IsInterface)
            {
                if (CanGetValue(obj, out var val, out var castExpression))
                {
                    var map = val.GetType().GetInterfaceMap(method.DeclaringType);
                    method = map.TargetMethods[Array.IndexOf(map.InterfaceMethods, method)];
                    realizedNode = Expression.Call(castExpression, method, args);
                }
            }
            return realizedNode ?? node;

        }

        private bool CanGetValue(Expression objExpr, out object value, out Expression castExpression)
        {
            if (objExpr is ConstantExpression constExpr)
            {
                value = constExpr.Value;
                castExpression = objExpr;
                return true;
            }
            else if (objExpr is UnaryExpression unaryExpr && unaryExpr.NodeType == ExpressionType.TypeAs)
            {
                return CanGetValue(unaryExpr.Operand, out value, out castExpression);
            }
            else if (objExpr is MemberExpression membExpr && membExpr.Member is FieldInfo field)
            {
                if (CanGetValue(membExpr.Expression, out var parent, out var uncastExpression))
                {
                    value = field.GetValue(parent);
                    var fieldExpr = Expression.Field(uncastExpression, field);
                    castExpression = Expression.TypeAs(fieldExpr, value.GetType());
                    return true;
                }
            }
            value = null;
            castExpression = null;
            return false;
        }

        private LambdaExpression GetExpressionEquivalent(MethodInfo method)
        {
            var bindingFlags = BindingFlags.Static
                | (method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic);

            var propertyName = GetPropertyName(method);
            var property = method.DeclaringType.GetProperty(propertyName, bindingFlags);
            if (property != null && FunctionMatchesExpressionType(method, property.PropertyType))
            {
                var expression = property.GetValue(null,null);
                return expression as LambdaExpression;
            }
            else
            {
                return null;
            }
        }

        private string GetPropertyName(MethodInfo method)
        {
            var name = method.Name;
            foreach (var p in method.GetParameters())
            {
                string typeName = GetTypeName(p.ParameterType);

                if (typeName != null)
                {
                    name += "_" + typeName;
                }

            }

            return name + "_Expression";
        }

        private string GetTypeName(Type type)
        {
            if (type.IsArray)
            {
                var rnk = type.GetArrayRank();
                var arrayWord = rnk == 1 ? "ArrayOf" : $"Array{rnk}Of";
                return arrayWord + GetTypeName(type.GetElementType());
            }
            else
            {
                return type.Name;
            }

            return null;

        }
        private Expression InlineExpression(LambdaExpression lambda, MethodCallExpression method)
        {
            var parameterValues = new Dictionary<ParameterExpression, Expression>();
            if (!method.Method.IsStatic)
            {
                parameterValues.Add(lambda.Parameters.First(), method.Object);
            }
            int offset = method.Method.IsStatic ? 0 : 1;
            for (int i = 0; i < method.Arguments.Count(); i++)
            {
                parameterValues.Add(lambda.Parameters[i+offset], method.Arguments[i]);
            }

            return new LambdaParameterInlinerVisitor(parameterValues).Visit(lambda.Body).Inline();
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
