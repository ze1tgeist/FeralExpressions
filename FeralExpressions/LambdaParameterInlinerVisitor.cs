using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions
{
    public class LambdaParameterInlinerVisitor : ExpressionVisitor
    {
        public LambdaParameterInlinerVisitor(Dictionary<ParameterExpression, Expression> parameterValues)
        {
            this.parameterValues = parameterValues;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (parameterValues.ContainsKey(node))
            {
                return parameterValues[node];
            }
            else
            {
                return base.VisitParameter(node);
            }
        }
        Dictionary<ParameterExpression, Expression> parameterValues;
    }
}
