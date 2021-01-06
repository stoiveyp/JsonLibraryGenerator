using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JsonLibraryGenerator
{
    public class NewtonsoftGenerator:JsonClassGenerator
    {
        protected override AttributeListSyntax JsonLibraryAttribute(AttributeSyntax propertyAttribute)
        {
            var propertyName = propertyAttribute.ArgumentList.Arguments.First().Expression.ToFullString();

            var argument = SyntaxFactory.AttributeArgument(
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Token(default, SyntaxKind.StringLiteralToken, propertyName, propertyName, default)));

            return SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(new[]
            {
                SyntaxFactory.Attribute(SyntaxFactory.ParseName("JsonProperty")).AddArgumentListArguments(argument)
            }));
        }
    }
}