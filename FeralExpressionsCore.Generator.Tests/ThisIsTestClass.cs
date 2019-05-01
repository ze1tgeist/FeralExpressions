using System;
using System.Collections.Generic;
using System.Text;

namespace FeralExpressionsCore.Generator.Tests
{
    public partial class ThisIsTestClass
    {
        public bool MethodWithThisIs() => this is ThisIsTestClass;
    }
}
