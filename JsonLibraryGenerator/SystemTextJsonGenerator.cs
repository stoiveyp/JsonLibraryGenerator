using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JsonLibraryGenerator
{
    public class SystemTextJsonGenerator : JsonClassGenerator
    {
        protected override AttributeListSyntax JsonLibraryAttribute(AttributeSyntax getJproperty)
        {
            throw new System.NotImplementedException();
        }
    }
}