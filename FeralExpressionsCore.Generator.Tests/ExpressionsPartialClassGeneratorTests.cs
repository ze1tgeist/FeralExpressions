using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressionsCore.Generator.Tests
{
    [TestClass]
    public class ExpressionsPartialClassGeneratorTests
    {

        [TestMethod]
        public void ExpressionPartialClassGenerator_converts_OuterPartialClass()
        {
            var sut = new ExpressionsPartialClassGenerator(new MethodToExpressionConverter(), ".expressions");

            var root = ReadRoot(@"OuterPartialClass.cs");

            var actual = sut.Generate(root)?.ToFullString();

            var expected = "using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Text;\r\nusing System.Threading.Tasks;\r\nusing System.Linq.Expressions;\r\n\r\nnamespace FeralExpressions.Test\r\n{\r\n    partial class  OuterPartialClass\r\n    {\r\n        partial class NestedPartialClass\r\n        {\r\n            public static Expression<Func<NestedPartialClass,int>> MethodInNestedPartial_Expression =>\r\n            (NestedPartialClass _this) => 0;\r\n        }\r\n        public static Expression<Func<OuterPartialClass,string>> MethodInOuterPartial_Expression =>\r\n        (OuterPartialClass _this) => \"abc\";\r\n        private static Expression<Func<OuterPartialClass,string>> PrivateMethodInOuterPartial_Expression =>\r\n        (OuterPartialClass _this) => \"abc\";\r\n        public static Expression<Func<OuterPartialClass,string,int,string,string>> MethodWithInterestingArgsInOuterPartial_Expression =>\r\n        (OuterPartialClass _this, string arg1, int arg2, string arg3) =>\r\n            $\"{arg1}+{arg2}+{arg3}\";\r\n        public static Expression<Func<OuterPartialClass,string>> MethodThatCallsAnInstanceMethod_Expression =>\r\n        (OuterPartialClass _this) =>\r\n            _this.MethodInOuterPartial();\r\n        public static Expression<Func<OuterPartialClass,string>> MethodThatCallsAStaticMethod_Expression =>\r\n        (OuterPartialClass _this) =>\r\n            StaticMethodInOuterPartial();\r\n        public static Expression<Func<string>> StaticMethodInOuterPartial_Expression =>\r\n        () => \"def\";\r\n    }\r\n}\r\n\r\n\r\n";

            Assert.AreEqual(expected, actual);
        }

        public void ExpressionPartialClassGenerator_converts_OuterNonPartialClass()
        {
            var sut = new ExpressionsPartialClassGenerator(new MethodToExpressionConverter(), ".expressions");

            var root = ReadRoot(@"OuterNonPartialClass.cs");

            var actual = sut.Generate(root)?.ToFullString();

            string expected = null;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ExpressionPartialClassGenerator_converts_GenericClass()
        {
            var sut = new ExpressionsPartialClassGenerator(new MethodToExpressionConverter(), ".expressions");

            var root = ReadRoot(@"GenericClass.cs");

            var actual = sut.Generate(root)?.ToFullString();

            string expected = "using System;\r\nusing System.Collections.Generic;\r\nusing System.Text;\r\nusing System.Linq.Expressions;\r\n\r\nnamespace FeralExpressionsCore.Generator.Tests\r\n{\r\n    public partial class GenericClassWithConstraints<T1, T2> where T1 : class\r\n    {\r\n        public static Expression<Func<GenericClassWithConstraints<T1,T2>,T1,T1>> NoChange_Expression =>\r\n        (GenericClassWithConstraints<T1,T2> _this, T1 arg) => arg;\r\n    }\r\n}\r\n";

            Assert.AreEqual(expected, actual);

        }

        private CompilationUnitSyntax ReadRoot(string path)
        {
            var code = ReadFile(path);
            var tree = CSharpSyntaxTree.ParseText(code);
            return (CompilationUnitSyntax)tree.GetRoot();

        }

        private static string ReadFile(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
