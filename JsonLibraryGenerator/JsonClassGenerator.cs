using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JsonLibraryGenerator
{
    public abstract class JsonClassGenerator
    {
        public string Generate(ClassDeclarationSyntax classToOverride)
        {
            var topOfTheTree = (CompilationUnitSyntax)classToOverride.Ancestors().Last();
            var nsName = classToOverride.Ancestors().OfType<NamespaceDeclarationSyntax>().First().Name;

            var cu = SyntaxFactory.CompilationUnit()
                .AddUsings(topOfTheTree.Usings.ToArray());
            var ns = SyntaxFactory.NamespaceDeclaration(nsName);

            var props = GetProperties(classToOverride);
            var newClass = SyntaxFactory.ClassDeclaration("Testing123") //CalculateCorrectName
                .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(classToOverride.Identifier.Text)))
                .AddModifiers(classToOverride.Modifiers.ToArray())
                .AddMembers(
                    props.Select(p =>
                        {
                            var newModifierList = new SyntaxTokenList(p.Modifiers.Where(m => m.Kind() != SyntaxKind.VirtualKeyword)
                                .Concat(new[] {SyntaxFactory.Token(SyntaxKind.OverrideKeyword)}));
                            return SyntaxFactory.PropertyDeclaration(p.Type, p.Identifier)
                                .WithModifiers(newModifierList)
                                .AddAttributeLists(JsonLibraryAttribute(GetJproperty(p)))
                                .AddAccessorListAccessors(p.AccessorList.Accessors.ToArray());
                        })
                        .Cast<MemberDeclarationSyntax>().ToArray());

            return cu.AddMembers(ns.AddMembers(newClass)).NormalizeWhitespace().ToFullString();
        }

        protected abstract AttributeListSyntax JsonLibraryAttribute(AttributeSyntax getJproperty);

        private IEnumerable<PropertyDeclarationSyntax> GetProperties(ClassDeclarationSyntax classToSupercede)
        {
            return classToSupercede.Members
                .OfType<PropertyDeclarationSyntax>()
                .Where(pds => pds.Modifiers.Any(m => m.Kind() == SyntaxKind.VirtualKeyword))
                .Where(pds => GetJproperty(pds) != null);
        }

        private AttributeSyntax GetJproperty(PropertyDeclarationSyntax syntax)
        {
            return syntax.AttributeLists.SelectMany(al => al.Attributes.ToArray())
                .FirstOrDefault(a => a.Name.ToFullString().Contains("JProperty"));
        }
    }
}