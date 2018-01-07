﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace FeralExpressions.Generator.Tests
{
    /// <summary>
    /// Summary description for MethodToExpressionRewriterTests
    /// </summary>
    [TestClass]
    public partial class MethodToExpressionConverterTests
    {
        [TestMethod]
        public void MethodToExpressionConverter_converts_static_expression_bodied_method_in_outer_partial_class_to_expression()
        {
            TestMethodToExpression
                (
                    codeFilePath: @"OuterPartialClass.cs",
                    methodName: "StaticMethodInOuterPartial",
                    expectedExpressionText: "        public static Expression<Func<string>> StaticMethodInOuterPartial_Expression =>\r\n        () => \"def\";\r\n"
                );
        }

        [TestMethod]
        public void MethodToExpressionConverter_converts_expression_bodied_method_in_outer_partial_class_to_expression()
        {
            TestMethodToExpression
                (
                    codeFilePath: @"OuterPartialClass.cs",
                    methodName: "MethodInOuterPartial",
                    expectedExpressionText: "        public static Expression<Func<OuterPartialClass,string>> MethodInOuterPartial_Expression =>\r\n        (OuterPartialClass _this) => \"abc\";\r\n"
                );
        }

        [TestMethod]
        public void MethodToExpressionConverter_converts_private_expression_bodied_method_in_outer_partial_class_to_expression()
        {
            TestMethodToExpression
                (
                    codeFilePath: @"OuterPartialClass.cs",
                    methodName: "PrivateMethodInOuterPartial",
                    expectedExpressionText: "        private static Expression<Func<OuterPartialClass,string>> PrivateMethodInOuterPartial_Expression =>\r\n        (OuterPartialClass _this) => \"abc\";\r\n"
                );
        } 

        [TestMethod]
        public void MethodToExpressionConverter_converts_expression_bodied_method_in_outer_partial_class_with_interesting_args_to_expression()
        {
            TestMethodToExpression
                (
                    codeFilePath: @"OuterPartialClass.cs",
                    methodName: "MethodWithInterestingArgsInOuterPartial",
                    expectedExpressionText: "        public static Expression<Func<OuterPartialClass,string,int,string,string>> MethodWithInterestingArgsInOuterPartial_Expression =>\r\n        (OuterPartialClass _this, string arg1, int arg2, string arg3) =>\r\n            $\"{arg1}+{arg2}+{arg3}\";\r\n"
                );
        }

        [TestMethod]
        public void MethodToExpressionConverter_converts_expression_bodied_method_in_nested_partial_class_to_expression()
        {
            TestMethodToExpression
                (
                    codeFilePath: @"OuterPartialClass.cs",
                    methodName: "MethodInNestedPartial",
                    expectedExpressionText: "            public static Expression<Func<NestedPartialClass,int>> MethodInNestedPartial_Expression =>\r\n            (NestedPartialClass _this) => 0;\r\n"
                );
        }

        [TestMethod]
        public void MethodToExpressionConverter_converts_expression_bodied_method_in_non_partial_class_to_null()
        {
            TestMethodToExpression
                (
                    codeFilePath: @"OuterNonPartialClass.cs",
                    methodName: "MethodInNonPartial",
                    expectedExpressionText: null
                );
        }

        [TestMethod]
        public void MethodToExpressionConverter_converts_expression_bodied_method_in__partial_class_with_non_partial_parent_to_null()
        {
            TestMethodToExpression
                (
                    codeFilePath: @"OuterNonPartialClass.cs",
                    methodName: "MethodInPartialInNonPartial",
                    expectedExpressionText: null
                );
        }

        [TestMethod]
        public void MethodToExpressionConverter_converts_expression_bodied_method_that_calls_an_instance_method()
        {
            TestMethodToExpression
                (
                    codeFilePath: @"OuterPartialClass.cs",
                    methodName: "MethodThatCallsAnInstanceMethod",
                    expectedExpressionText: "        public static Expression<Func<OuterPartialClass,string>> MethodThatCallsAnInstanceMethod_Expression =>\r\n        (OuterPartialClass _this) =>\r\n            _this.MethodInOuterPartial();\r\n"
                );
        }

        [TestMethod]
        public void MethodToExpressionConverter_converts_expression_bodied_method_that_calls_a_static_method()
        {
            TestMethodToExpression
                (
                    codeFilePath: @"OuterPartialClass.cs",
                    methodName: "MethodThatCallsAStaticMethod",
                    expectedExpressionText: "        public static Expression<Func<OuterPartialClass,string>> MethodThatCallsAStaticMethod_Expression =>\r\n        (OuterPartialClass _this) =>\r\n            StaticMethodInOuterPartial();\r\n"
                );
        }

        [TestMethod]
        public void MethodToExpressionConverter_converts_extension_method()
        {
            TestMethodToExpression
                (
                    codeFilePath: @"ExtensionMethods.cs",
                    methodName: "Append",
                    expectedExpressionText: "        public static Expression<Func<string,string,string>> Append_Expression =>\r\n        (string str, string arg) => str + arg;\r\n"
                );
        }


        private void TestMethodToExpression(string codeFilePath, string methodName, string expectedExpressionText)
        {
            var sut = new MethodToExpressionConverter();

            var root = ReadRoot(codeFilePath);
            var method = GetMethod(methodName, root);

            var actual = sut.Convert(method)?.ToFullString();

            Assert.AreEqual(expectedExpressionText, actual);

        }

        private MethodDeclarationSyntax GetMethod(string methodName, CompilationUnitSyntax root)
        {
            return
                root
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Where(m => m.Identifier.Text == methodName)
                    .FirstOrDefault();
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
