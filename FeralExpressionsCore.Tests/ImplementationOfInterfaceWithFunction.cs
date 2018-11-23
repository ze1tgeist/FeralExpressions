using System;
using System.Collections.Generic;
using System.Text;

namespace FeralExpressionsCore.Tests
{
    partial class ImplementationOfInterfaceWithFunction : IInterfaceWithFunction
    {
        private string para;
        public ImplementationOfInterfaceWithFunction(string para)
        {
            this.para = para;
        }
        public string Function(string arg) => this.para + " " + arg;
    }

    partial class RecursiveImplementationOfInterfaceWithFunction : IInterfaceWithFunction
    {
        public IInterfaceWithFunction inner;
        public RecursiveImplementationOfInterfaceWithFunction(IInterfaceWithFunction inner)
        {
            this.inner = inner;
        }
        public string Function(string arg) => this.inner.Function(arg);
    }
}
