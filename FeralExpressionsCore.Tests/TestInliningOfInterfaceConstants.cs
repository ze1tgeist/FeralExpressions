using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace FeralExpressionsCore.Tests
{
    public class TestInliningOfInterfaceConstants
    {
        [Fact]
        public void InterfaceConstantsAreRealizedCorrectly()
        {
            IInterfaceWithFunction impl = new ImplementationOfInterfaceWithFunction("def");

            Expression<Func<string>> exBasedOnInterface = () => impl.Function("abc");

            var actualInlinedExpression = exBasedOnInterface.Inline();

            var lambda = actualInlinedExpression as LambdaExpression;
            Assert.NotNull(lambda);
            var body = lambda.Body as BinaryExpression;
            Assert.NotNull(body);
            Assert.Equal(ExpressionType.Add, body.NodeType);
            var right = body.Right as ConstantExpression;
            Assert.NotNull(right);
            Assert.Equal("abc", right.Value);
            var left = body.Left as BinaryExpression;
            Assert.NotNull(left);
            Assert.Equal(ExpressionType.Add, left.NodeType);
            var space = left.Right as ConstantExpression;
            Assert.NotNull(space);
            Assert.Equal(" ", space.Value);

            var field = left.Left as MemberExpression;
            Assert.NotNull(field);
            Assert.Equal("para", field.Member.Name);
        }
    }
}
