using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FeralExpressions.Generator
{
    class ThisOrImplicitThisTo_ThisRewriter : CSharpSyntaxRewriter
    {

        public override SyntaxNode VisitThisExpression(ThisExpressionSyntax node)
        {
            return SyntaxFactory.IdentifierName("_this");
        }
    }
}
