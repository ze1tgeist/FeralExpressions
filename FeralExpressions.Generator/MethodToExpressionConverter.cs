using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressions.Generator
{
    public class MethodToExpressionConverter
    {
        public PropertyDeclarationSyntax Convert(MethodDeclarationSyntax method, SemanticModel semanticModel = null)
        {
            if (method.Body == null && method.ExpressionBody != null && AreAllParentClassesPartial(method))
            {
                ISymbol methodSymbol = null;
                if (semanticModel != null)
                {
                    methodSymbol = semanticModel.GetDeclaredSymbol(method);
                }
                var parentClass = (method.Parent as ClassDeclarationSyntax);
                var methodIsStatic = method.Modifiers.Any(SyntaxKind.StaticKeyword);
                var funcParamTypes =
                    (
                        methodIsStatic
                            ? new TypeSyntax[] { }
                            : Enumerable.Repeat(SyntaxFactory.ParseTypeName(parentClass.Identifier.Text), 1)
                         
                    ).Union(
                        method.ParameterList.Parameters.AsEnumerable()
                        .Select(p => p.Type.WithoutTrailingTrivia())
                    )
                    .Union(
                        Enumerable.Repeat(method.ReturnType.WithoutTrailingTrivia(), 1)
                    );

                var funcTypesSeperatedList = SyntaxFactory.SeparatedList<TypeSyntax>(funcParamTypes);
                var funcTypedArgumentList = SyntaxFactory.TypeArgumentList(funcTypesSeperatedList);
                var funcIdentifier = SyntaxFactory.Identifier("Func");
                var funcType = SyntaxFactory.GenericName(funcIdentifier, funcTypedArgumentList);

                var expressionIdentifier = SyntaxFactory.Identifier("Expression");
                var expressionTypesSeperatedList = SyntaxFactory.SeparatedList<TypeSyntax>(Enumerable.Repeat(funcType, 1));
                var expressionTypedArgumentList = SyntaxFactory.TypeArgumentList(expressionTypesSeperatedList);
                var expressionType = SyntaxFactory.GenericName(expressionIdentifier, expressionTypedArgumentList).WithTriviaFrom(method.ReturnType);

                var lambdaParameters =
                    methodIsStatic
                        ? method.ParameterList
                        : SyntaxFactory.ParameterList(BuildStaticParamsWith_this(method, parentClass)).WithTrailingTrivia(SyntaxFactory.Space);
                var methodExpression = (CSharpSyntaxNode)new ThisOrImplicitThisTo_ThisRewriter(semanticModel, methodSymbol).Visit(method.ExpressionBody.Expression).WithLeadingTrivia(method.ExpressionBody.Expression.GetLeadingTrivia());
                var lamdaExpression = SyntaxFactory.ParenthesizedLambdaExpression(
                    SyntaxFactory.Token(SyntaxKind.None),
                    lambdaParameters.WithLeadingTrivia(SyntaxFactory.CarriageReturn,SyntaxFactory.LineFeed,method.GetLeadingTrivia().Last()),
                    method.ExpressionBody.ArrowToken,
                    methodExpression);
                var propertyExpressionBody = SyntaxFactory.ArrowExpressionClause
                    (
                        SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), 
                        lamdaExpression
                    );
                var modifiers = method.Modifiers;
                if (!modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    modifiers = SyntaxFactory.TokenList(modifiers.Union(Enumerable.Repeat(SyntaxFactory.Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(SyntaxFactory.Space), 1)));
                }

                var propertyDec = SyntaxFactory.PropertyDeclaration(
                    method.AttributeLists,
                    modifiers,
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

        private SeparatedSyntaxList<ParameterSyntax> BuildStaticParamsWith_this(MethodDeclarationSyntax method, ClassDeclarationSyntax parentClass)
        {
            var parameters = Enumerable.Repeat(SyntaxFactory.Parameter(
                        SyntaxFactory.List<AttributeListSyntax>(),
                        SyntaxFactory.TokenList(),
                        SyntaxFactory.ParseTypeName(parentClass.Identifier.Text).WithTrailingTrivia(SyntaxFactory.Space),
                        SyntaxFactory.Identifier("_this"),
                        null
                    ), 1).Union(method.ParameterList.Parameters);
            return
                SyntaxFactory.SeparatedList<ParameterSyntax>(parameters, Enumerable.Repeat(SyntaxFactory.Token(SyntaxKind.CommaToken).WithTrailingTrivia(SyntaxFactory.Space), parameters.Count()-1));
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
