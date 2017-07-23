using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions.Test
{
    partial class  OuterPartialClass
    {
        partial class NestedPartialClass
        {
            public int MethodInNestedPartial() => 0;
        }

        class NestedNonPartialClass
        {
            public int MethodInNestedNonPartial() => 321;
        }

        public string MethodInOuterPartial() => "abc";
        private string PrivateMethodInOuterPartial() => "abc";

        public string MethodWithInterestingArgsInOuterPartial(string arg1, int arg2, string arg3) =>
            $"{arg1}+{arg2}+{arg3}";
    }
}


