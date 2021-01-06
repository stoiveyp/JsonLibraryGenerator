using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class JsonSwitchReceiver : ISyntaxReceiver
{
    public ClassDeclarationSyntax ClassToSupercede { get; set; }

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is AttributeSyntax attribute && attribute.Name.ToFullString().Contains("JsonSwitch"))
        {
            ClassToSupercede = attribute.Parent.Parent as ClassDeclarationSyntax;
        }
    }
}