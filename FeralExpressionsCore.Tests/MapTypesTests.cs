using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace FeralExpressionsCore.Tests
{
    public class MappingTests
    {
        private interface IFace
        {

        }
        private class Face: IFace
        {

        }
        [Fact]
        public void Enumerable_Empty_is_corretly_mapped()
        {
            var mapping = new Dictionary<Type, Type> { { typeof(IFace), typeof(Face) } };
            Expression<Func<IEnumerable<IFace>>> expression = () => Enumerable.Empty<IFace>();

            var actual = expression.MapTypes(mapping);

            var actualBody = (actual as LambdaExpression)?.Body;
            var actualMethod = (actualBody as MethodCallExpression)?.Method;
            Assert.NotNull(actualMethod);

            var actualTypeArgument = actualMethod.GetGenericArguments().First();
            Assert.Equal(typeof(Face), actualTypeArgument);
        }
    }
}
