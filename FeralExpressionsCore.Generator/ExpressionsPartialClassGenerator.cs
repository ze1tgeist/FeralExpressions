using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressionsCore.Generator
{
    public class ExpressionsPartialClassGenerator
    {
        public ExpressionsPartialClassGenerator(MethodToExpressionConverter methodConverter, string extensionPrefix)
        {
            this.methodConverter = methodConverter;
            this.extensionPrefix = extensionPrefix;
        }

        public string GenerateFile(string csPath)
        {
            (var root, var semanticModel) = ReadRoot(csPath);

            var expressionsRoot = Generate(root, semanticModel);
            if (expressionsRoot != null)
            {
                var expressionsCsPath = System.IO.Path.ChangeExtension(csPath, extensionPrefix + ".cs");

                using (var writer = new StreamWriter(expressionsCsPath))
                {
                    writer.Write(expressionsRoot.ToFullString());
                }
                return expressionsCsPath;
            }
            else
                return null;
        }

        public CompilationUnitSyntax Generate(CompilationUnitSyntax sourceRoot, SemanticModel semanticModel = null)
        {
            var methodCounts =
                sourceRoot.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .GroupBy(m => new { m.Parent, m.Identifier.Text })
                .ToDictionary(grp => grp.Key, grp => 0);

            var methodExpressions =
                sourceRoot.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Select(m => new MethodExpression()
                {
                    Method = m,
                    Expression = methodConverter.Convert(m, methodCounts[new { m.Parent, m.Identifier.Text }]++, semanticModel)
                })
                .Where(pc => pc.Expression != null)
                .ToList();

            if (methodExpressions.Any())
            {
                return (CompilationUnitSyntax)ConvertNode(sourceRoot, methodExpressions);
            }
            else
            {
                return null;
            }
        }


        private SyntaxNode ConvertNode(SyntaxNode oldNode, List<MethodExpression> expressionsStillToHome)
        {
            if (oldNode is CompilationUnitSyntax root)
            {
                return root
                    .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(GetMembers(root, expressionsStillToHome)))
                    .WithUsings(
                        AddUsingsIfRequired(root.Usings)
                     );
            }
            if (oldNode is ClassDeclarationSyntax classDec)
            {
                return classDec
                    .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(GetMembers(classDec, expressionsStillToHome)));
            }
            if (oldNode is NamespaceDeclarationSyntax namespaceDec)
            {
                return namespaceDec
                    .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(GetMembers(namespaceDec, expressionsStillToHome)));
            }
            else
            {
                return null;
            }
        }

        private SyntaxList<UsingDirectiveSyntax> AddUsingsIfRequired(SyntaxList<UsingDirectiveSyntax> original)
        {
            var usedNames =
                original
                .Select(u => u.Name.ToString())
                .ToArray();
            var usings = original;
            if (!usedNames.Contains("System"))
            {
                usings = usings.Add(CreateUsingSyntax("System"));
            }
            if (!usedNames.Contains("System.Linq.Expressions"))
            {
                usings = usings.Add(CreateUsingSyntax("System.Linq.Expressions"));
            }

            return usings;
        }

        private UsingDirectiveSyntax CreateUsingSyntax(string name)
        {
            var nameSyntax = 
                name.Split(".")
                .Select(nm => SyntaxFactory.IdentifierName(nm))
                .Aggregate((NameSyntax)null, (acc, nm) => acc == null ? nm : SyntaxFactory.QualifiedName(acc, nm));

            return
                SyntaxFactory.UsingDirective(
                    nameSyntax.WithLeadingTrivia(SyntaxFactory.Space)
                )
                .WithTrailingTrivia(SyntaxFactory.EndOfLine("\r\n"));
        }

        private IEnumerable<MemberDeclarationSyntax> GetMembers(SyntaxNode oldNode, List<MethodExpression> expressionsStillToHome)
        {
            int i = 0;
            while (i < expressionsStillToHome.Count())
            {
                var me = expressionsStillToHome[i];

                if (me.Method.Parent == oldNode)
                {
                    expressionsStillToHome.RemoveAt(i);
                    yield return me.Expression;
                }
                else if (me.Method.Ancestors().Contains(oldNode))
                {
                    yield return (MemberDeclarationSyntax)ConvertNode(me.Method.Ancestors().TakeWhile(a => a != oldNode).Last(), expressionsStillToHome);
                }
                else
                {
                    i += 1;
                }
            }
        }

        private class MethodExpression
        {
            public MemberDeclarationSyntax Method { get; set; }
            public MemberDeclarationSyntax Expression { get; set; }
        }

        private (CompilationUnitSyntax root, SemanticModel semanticModel) ReadRoot(string path)
        {
            var code = ReadFile(path);
            var tree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation
                .Create("assembly" + Guid.NewGuid().ToString("N")) // 32 digits, no hyphens or braces
                .AddSyntaxTrees(tree);

            return ((CompilationUnitSyntax)tree.GetRoot(), compilation.GetSemanticModel(tree));

        }

        private static string ReadFile(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadToEnd();
            }
        }

        private MethodToExpressionConverter methodConverter;
        private string extensionPrefix;
    }
}
