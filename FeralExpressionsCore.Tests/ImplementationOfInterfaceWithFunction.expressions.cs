using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace FeralExpressionsCore.Tests
{
    partial class ImplementationOfInterfaceWithFunction : IInterfaceWithFunction
    {
        public static Expression<Func<ImplementationOfInterfaceWithFunction,string,string>> Function_Expression =>
        (ImplementationOfInterfaceWithFunction _this, string arg) => _this.para + " " + arg;
    }

    partial class RecursiveImplementationOfInterfaceWithFunction : IInterfaceWithFunction
    {
        public static Expression<Func<RecursiveImplementationOfInterfaceWithFunction,string,string>> Function_Expression =>
        (RecursiveImplementationOfInterfaceWithFunction _this, string arg) => _this.inner.Function(arg);
    }
}
