using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions
{
    public class MethodToExpressionConverter
    {
        public PropertyDeclarationSyntax Convert(MethodDeclarationSyntax method)
        {
            if (method.Body == null && method.ExpressionBody != null && AreAllParentClassesPartial(method))
            {
                var funcParamTypes =
                    method.ParameterList.Parameters.AsEnumerable()
                    .Select(p => p.Type.WithoutTrailingTrivia())
                    .Union(Enumerable.Repeat(method.ReturnType.WithoutTrailingTrivia(), 1));

                var funcTypesSeperatedList = SyntaxFactory.SeparatedList<TypeSyntax>(funcParamTypes);
                var funcTypedArgumentList = SyntaxFactory.TypeArgumentList(funcTypesSeperatedList);
                var funcIdentifier = SyntaxFactory.Identifier("Func");
                var funcType = SyntaxFactory.GenericName(funcIdentifier, funcTypedArgumentList);

                var expressionIdentifier = SyntaxFactory.Identifier("Expression");
                var expressionTypesSeperatedList = SyntaxFactory.SeparatedList<TypeSyntax>(Enumerable.Repeat(funcType, 1));
                var expressionTypedArgumentList = SyntaxFactory.TypeArgumentList(expressionTypesSeperatedList);
                var expressionType = SyntaxFactory.GenericName(expressionIdentifier, expressionTypedArgumentList).WithTriviaFrom(method.ReturnType);

                var methodExpression = method.ExpressionBody.Expression;
                var lamdaExpression = SyntaxFactory.ParenthesizedLambdaExpression(
                    SyntaxFactory.Token(SyntaxKind.None),
                    method.ParameterList.WithLeadingTrivia(SyntaxFactory.CarriageReturn,SyntaxFactory.LineFeed,method.GetLeadingTrivia().Last()),
                    method.ExpressionBody.ArrowToken,
                    methodExpression);
                var propertyExpressionBody = SyntaxFactory.ArrowExpressionClause
                    (
                        SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), 
                        lamdaExpression
                    );

                var propertyDec = SyntaxFactory.PropertyDeclaration(
                    method.AttributeLists,
                    method.Modifiers,
                    expressionType,
                    null,
                    SyntaxFactory.Identifier(method.Identifier.Text + "_Expression").WithTrailingTrivia(SyntaxFactory.Space),
                    null,
                    propertyExpressionBody,
                    null,
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken).WithTrailingTrivia(SyntaxFactory.CarriageReturn, SyntaxFactory.LineFeed)
                ).WithLeadingTrivia(method.GetLeadingTrivia().Last());
                return propertyDec;
            }
            else
                return null;

        }

        private bool AreAllParentClassesPartial(MemberDeclarationSyntax member)
        {
            if (member is MethodDeclarationSyntax method && method.Parent is ClassDeclarationSyntax parentClass)
            {
                return AreAllParentClassesPartial(parentClass);
            }
            else if (member is ClassDeclarationSyntax classDec && classDec.Parent is ClassDeclarationSyntax classDecsParentClass)
            {
                return
                    classDec.Modifiers.Any(SyntaxKind.PartialKeyword)
                    && AreAllParentClassesPartial(classDecsParentClass);
            }
            else if (member is ClassDeclarationSyntax classDec1 && classDec1.Parent is NamespaceDeclarationSyntax)
            {
                return
                    classDec1.Modifiers.Any(SyntaxKind.PartialKeyword);
            }
            else
                return false;

        }
    }
}
