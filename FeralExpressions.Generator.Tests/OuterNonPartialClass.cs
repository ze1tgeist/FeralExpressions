using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions.Test
{
    public class OuterNonPartialClass
    {
        partial class InnerPartialClass
        {
            int MethodInPartialInNonPartial() => 0;
        }

        int MethodInNonPartial() => 1;
    }
}
