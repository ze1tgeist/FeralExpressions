using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions.Test
{
    public static partial class ExtensionMethods
    {
        public static string Append(this string str, string arg) => str + arg;
    }
}
