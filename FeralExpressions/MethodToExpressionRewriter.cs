using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FeralExpressions
{
    public class MethodToExpressionRewriter : CSharpSyntaxRewriter
    {
        public MethodToExpressionRewriter(MethodToExpressionConverter methodToExpressionConverter)
        {
            this.methodToExpressionConverter = methodToExpressionConverter;
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            //if (node.Body == null && node.ExpressionBody != null)
            //{
            //    return ConvertMethodToExpressionProperty(node);
            //}
            //else
            //{
            //    return node;
            //}
            return node;
        }

        public override SyntaxList<TNode> VisitList<TNode>(SyntaxList<TNode> list)
        {

            if (typeof(TNode) == typeof(MemberDeclarationSyntax))
            {
                var newList = new List<TNode>(list);


                foreach (TNode node in list)
                {
                    var member = node as MemberDeclarationSyntax;

                    if (member is MethodDeclarationSyntax method && method.Body == null && method.ExpressionBody != null)
                    {
                        var expressionProperty = methodToExpressionConverter.Convert(method);
                        newList.Add(expressionProperty as TNode);
                    }
                }
                if (newList.Count() != list.Count())
                {
                    newList[list.Count() - 1] = newList[list.Count() - 1].WithTrailingTrivia(SyntaxFactory.LineFeed, SyntaxFactory.LineFeed);
                }

                return base.VisitList(SyntaxFactory.List<TNode>(newList));
            }
            else
            {
                return base.VisitList(list);
            }
        }


        private MethodToExpressionConverter methodToExpressionConverter;
    }
}
