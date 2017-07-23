using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions.Generator.Tests
{
    [TestClass]
    public class ExpressionsPartialClassGeneratorTests
    {

        [TestMethod]
        public void ExpressionPartialClassGenerator_converts_OuterPartialClass()
        {
            var sut = new ExpressionsPartialClassGenerator(new MethodToExpressionConverter());

            var root = ReadRoot(@"OuterPartialClass.cs");

            var actual = sut.Generate(root)?.ToFullString();

            var expected = "using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Text;\r\nusing System.Threading.Tasks;\r\nusing System.Linq.Expressions;\r\n\r\nnamespace FeralExpressions.Test\r\n{\r\n    partial class  OuterPartialClass\r\n    {\r\n        partial class NestedPartialClass\r\n        {\r\n            public Expression<Func<int>> MethodInNestedPartial_Expression =>\r\n            () => 0;\r\n        }\r\n        public Expression<Func<string>> MethodInOuterPartial_Expression =>\r\n        () => \"abc\";\r\n        private Expression<Func<string>> PrivateMethodInOuterPartial_Expression =>\r\n        () => \"abc\";\r\n        public Expression<Func<string,int,string,string>> MethodWithInterestingArgsInOuterPartial_Expression =>\r\n        (string arg1, int arg2, string arg3) =>\r\n            $\"{arg1}+{arg2}+{arg3}\";\r\n    }\r\n}\r\n\r\n\r\n";

            Assert.AreEqual(expected, actual);
        }

        public void ExpressionPartialClassGenerator_converts_OuterNonPartialClass()
        {
            var sut = new ExpressionsPartialClassGenerator(new MethodToExpressionConverter());

            var root = ReadRoot(@"OuterNonPartialClass.cs");

            var actual = sut.Generate(root)?.ToFullString();

            string expected = null;

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
