using System;
using System.Collections.Generic;
using System.Text;

namespace FeralExpressionsCore.Generator.Tests
{
    public partial class ParameterTypesTestClass
    {
        public System.String SimpleName(String str) => str;
        public System.String QualifiedName(System.Text.RegularExpressions.Regex str) => "abc";
        public System.String ArrayName(System.Text.RegularExpressions.Regex[] str) => "abc";
        public System.String DoubleArrayName(System.Text.RegularExpressions.Regex[,] str) => "abc";
        public System.String RaggedArrayName(System.Text.RegularExpressions.Regex[,][] str) => "abc";
        public System.String NullableName(DateTime? str) => "abc";
        public System.String PredefinedStringName(string str) => "abc";
        public System.String PredefinedIntName(int i) => "abc";
    }
}
