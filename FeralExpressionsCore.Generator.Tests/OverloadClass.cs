using System;
using System.Collections.Generic;
using System.Text;

namespace FeralExpressionsCore.Generator.Tests
{
    public partial class OverloadClass
    {
        public partial class Container1
        {
            public string Method(string arg1, string arg2) => arg1;
            public string Method(string arg1) => arg1;
        }

        public partial class Container2
        {
            public string Method(string arg1, string arg2) => arg1;

        }

        public string Method(string arg1, string arg2) => arg1;
        public string Method(string arg1) => arg1;

    }
}
