using FeralExpressionsCore.ReproduceRecursionBug;
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

            InlineAndCheckExpression(exBasedOnInterface);
        }

        [Fact]
        public void RecursiveOnceInterfaceConstantsAreRealizedCorrectly()
        {
            IInterfaceWithFunction impl = 
                new RecursiveImplementationOfInterfaceWithFunction(
                    new ImplementationOfInterfaceWithFunction("def"));

            Expression<Func<string>> exBasedOnInterface = () => impl.Function("abc");

            var actualInlinedExpression = exBasedOnInterface.Inline();

        }

        [Fact]
        public void RecursiveTwiceInterfaceConstantsAreRealizedCorrectly()
        {
            IInterfaceWithFunction impl = 
                new RecursiveImplementationOfInterfaceWithFunction(
                    new RecursiveImplementationOfInterfaceWithFunction(
                        new ImplementationOfInterfaceWithFunction("def")));

            Expression<Func<string>> exBasedOnInterface = () => impl.Function("abc");

            var actualInlinedExpression = exBasedOnInterface.Inline();

        }

        private void InlineAndCheckExpression(Expression expression)
        {
            Expression actualInlinedExpression = expression.Inline();
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

        [Fact]
        public void RealizationOfInterfacesCopesWithMoreComplexModels()
        {
            ReproduceRecursionBug.IConverter<ReproduceRecursionBug.Models.Legislation, ReproduceRecursionBug.Dtos.Legislation> converter =
                new LegislationModelToDtoConverter();
            ReproduceRecursionBug.IDomainToDtoMapper<ReproduceRecursionBug.Models.Legislation, ReproduceRecursionBug.Dtos.Legislation> mapper =
                new IndependentSolutionDomainToDtoMapper<ReproduceRecursionBug.Models.Legislation, ReproduceRecursionBug.Dtos.Legislation>(converter);


            Expression<Func<ReproduceRecursionBug.Models.Legislation, ReproduceRecursionBug.Dtos.Legislation>> expr = l => mapper.DomainEntityToDto(l);

            var inlinedExpression = expr.Inline();

            var lambdaExpression = inlinedExpression as LambdaExpression;
            Assert.NotNull(lambdaExpression);

            var body = lambdaExpression.Body;

            var methodCall = body as MethodCallExpression;

            // it is supposed to have replaced the method Convert with an expression.  Check that methodCall is null shows this.
            Assert.Null(methodCall);
        }
    }
}
