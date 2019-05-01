using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FeralExpressionsCore.Generator
{
    class ThisOrImplicitThisTo_ThisRewriter : CSharpSyntaxRewriter
    {
        public ThisOrImplicitThisTo_ThisRewriter(SemanticModel semanticModel, ISymbol methodBeingParsedSymbol)
        {
            this.semanticModel = semanticModel;
            this.methodBeingParsedSymbol = methodBeingParsedSymbol;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax invocation)
        {
            if (invocation.Expression is SimpleNameSyntax name)
            {
                if (semanticModel != null && methodBeingParsedSymbol != null)
                {
                    var symbol = semanticModel.GetSymbolInfo(invocation.Expression);
                    if (symbol.Symbol != null && !symbol.Symbol.IsStatic)
                    {
                        if (symbol.Symbol.ContainingType == methodBeingParsedSymbol.ContainingType)
                        {
                            return invocation
                                .WithExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("_this"),
                                        name
                                    )
                                );
                        }
                    }
                }

            }
            return base.VisitInvocationExpression(invocation);
        }

        public override SyntaxNode VisitThisExpression(ThisExpressionSyntax node)
        {
            return SyntaxFactory.IdentifierName("_this");
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            if (node.OperatorToken.Kind() == SyntaxKind.IsKeyword)
            {
                node = node.WithOperatorToken(node.OperatorToken.WithLeadingTrivia(SyntaxFactory.Space));
            }
            return base.VisitBinaryExpression(node);
        }

        private SemanticModel semanticModel;
        private ISymbol methodBeingParsedSymbol;
    }
}
