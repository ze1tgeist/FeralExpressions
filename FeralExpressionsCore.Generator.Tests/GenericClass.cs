using System;
using System.Collections.Generic;
using System.Text;

namespace FeralExpressionsCore.Generator.Tests
{
    public partial class GenericClassWithConstraints<T1, T2> where T1 : class
    {
        public T1 NoChange(T1 arg) => arg;
    }
}
