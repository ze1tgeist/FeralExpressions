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
}
