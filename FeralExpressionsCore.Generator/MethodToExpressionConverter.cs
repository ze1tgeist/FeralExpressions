using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralExpressionsCore.Generator
{
    public class MethodToExpressionConverter
    {
        public ClassDeclarationSyntax Convert(MethodDeclarationSyntax method, SemanticModel semanticModel = null)
        {
            if (method.Body == null && method.ExpressionBody != null && AreAllParentClassesPartial(method))
            {
                var indent = method.GetLeadingTrivia().Last();

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
                            : Enumerable.Repeat(GetTypeOfClass(parentClass), 1)
                         
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
                    SyntaxFactory.ParameterList(
                        methodIsStatic
                        ? RemoveThisFromExtensionMethod(method)
                        : BuildStaticParamsWith_this(method, parentClass)
                    ).WithTrailingTrivia(SyntaxFactory.Space);
                var methodExpression = (CSharpSyntaxNode)new ThisOrImplicitThisTo_ThisRewriter(semanticModel, methodSymbol).Visit(method.ExpressionBody.Expression).WithLeadingTrivia(method.ExpressionBody.Expression.GetLeadingTrivia());
                var lamdaExpression = SyntaxFactory.ParenthesizedLambdaExpression(
                    SyntaxFactory.Token(SyntaxKind.None),
                    lambdaParameters.WithLeadingTrivia(SyntaxFactory.CarriageReturn,SyntaxFactory.LineFeed,indent, SyntaxFactory.Tab, SyntaxFactory.Tab),
                    method.ExpressionBody.ArrowToken,
                    methodExpression);
                var propertyExpressionBody = SyntaxFactory.ArrowExpressionClause
                    (
                        SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), 
                        lamdaExpression
                    );
                var kindsToRemove = new SyntaxKind[] { SyntaxKind.VirtualKeyword, SyntaxKind.NewKeyword, SyntaxKind.OverrideKeyword };
                var modifiers = 
                    SyntaxFactory.TokenList(method.Modifiers.Where(t => !kindsToRemove.Contains(t.Kind()))) ;

                modifiers = SyntaxFactory.TokenList(modifiers.Select(m => m.WithLeadingTrivia().WithTrailingTrivia(SyntaxFactory.Space)));
                if (!modifiers.Any(SyntaxKind.StaticKeyword))
                {
                    modifiers = SyntaxFactory.TokenList(modifiers.Union(Enumerable.Repeat(SyntaxFactory.Token(SyntaxKind.StaticKeyword).WithTrailingTrivia(SyntaxFactory.Space), 1)));
                }


                var propertyDec = SyntaxFactory.PropertyDeclaration(
                    method.AttributeLists,
                    modifiers,
                    expressionType,
                    null,
                    SyntaxFactory.Identifier("Expression").WithTrailingTrivia(SyntaxFactory.Space),
                    null,
                    propertyExpressionBody,
                    null,
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken).WithTrailingTrivia(SyntaxFactory.CarriageReturn, SyntaxFactory.LineFeed)
                ).WithLeadingTrivia(indent, SyntaxFactory.Tab);
                //return propertyDec;

                var members = SyntaxFactory.List<MemberDeclarationSyntax>(new MemberDeclarationSyntax[] { propertyDec });
                var classDesc = SyntaxFactory.ClassDeclaration(method.Identifier.Text + "_ExpressionClass")
                    .WithKeyword(
                        SyntaxFactory.Token(SyntaxKind.ClassKeyword)
                        .WithTrailingTrivia(SyntaxFactory.Space))
                    .WithOpenBraceToken(IndentedOnOwnLine(SyntaxKind.OpenBraceToken, indent))
                    .WithCloseBraceToken(IndentedOnOwnLine(SyntaxKind.CloseBraceToken, indent))
                    .WithModifiers(modifiers)
                    .WithLeadingTrivia(indent)
                    .WithMembers(members);

                return classDesc;
            }
            else
                return null;

        }

        private SyntaxToken IndentedOnOwnLine(SyntaxKind syntaxKind, SyntaxTrivia indent)
        {
            return
                SyntaxFactory.Token(syntaxKind)
                .WithLeadingTrivia(SyntaxFactory.CarriageReturn, SyntaxFactory.LineFeed, indent)
                .WithTrailingTrivia(SyntaxFactory.CarriageReturn, SyntaxFactory.LineFeed);
        }

        private TypeSyntax GetTypeOfClass(ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration.TypeParameterList != null && classDeclaration.TypeParameterList.Parameters.Any())
            {
                var typesForArguments = classDeclaration.TypeParameterList.Parameters.Select(ps => SyntaxFactory.ParseTypeName(ps.Identifier.Text));
                var seperatedArguments = SyntaxFactory.SeparatedList<TypeSyntax>(typesForArguments);
                var argumentList = SyntaxFactory.TypeArgumentList(seperatedArguments);
                return SyntaxFactory.GenericName(classDeclaration.Identifier, argumentList);
            }
            else
                return SyntaxFactory.ParseTypeName(classDeclaration.Identifier.Text);
        }
        private SeparatedSyntaxList<ParameterSyntax> RemoveThisFromExtensionMethod(MethodDeclarationSyntax method)
        {
            var parameters =
                method
                .ParameterList
                .Parameters
                .Select(p => RemoveThis(p));

            return
                parameters.Count() > 0
                ? SyntaxFactory.SeparatedList<ParameterSyntax>(parameters, Enumerable.Repeat(SyntaxFactory.Token(SyntaxKind.CommaToken).WithTrailingTrivia(SyntaxFactory.Space), parameters.Count() - 1))
                : SyntaxFactory.SeparatedList<ParameterSyntax>();
        }

        private ParameterSyntax RemoveThis(ParameterSyntax parameter)
        {
            var modifiers = from m in parameter.Modifiers where m.Text != "this"  select m;
            var modifiersList = SyntaxFactory.TokenList(modifiers);

            return SyntaxFactory.Parameter(parameter.AttributeLists, modifiersList, parameter.Type, parameter.Identifier, parameter.Default);
        }

        private SeparatedSyntaxList<ParameterSyntax> BuildStaticParamsWith_this(MethodDeclarationSyntax method, ClassDeclarationSyntax parentClass)
        {
            var parameters = Enumerable.Repeat(SyntaxFactory.Parameter(
                        SyntaxFactory.List<AttributeListSyntax>(),
                        SyntaxFactory.TokenList(),
                        GetTypeOfClass(parentClass).WithTrailingTrivia(SyntaxFactory.Space),
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
