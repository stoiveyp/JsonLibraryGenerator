using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JsonLibraryGenerator
{
    [Generator]
    public class JsonLibraryGenerator:ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new JsonSwitchReceiver());
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
        }

        private static readonly DiagnosticDescriptor NewtonsoftExists = new("JsonSwitch001N",
            "Generating using Newtonsoft",
            "Found references to both Newtonsoft and System.Text.Json - using Newtonsoft",
            "JsonSwitch",
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor SystemTextJsonExists = new("JsonSwitch001S",
            "Generating using System.Text.Json",
            "Found references to System.Text.Json only - using System.Text.Json",
            "JsonSwitch",
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor NeitherFramework = new("JsonSwitch002",
            "No JSON Preference found",
            "Found no references to either Newtonsoft and System.Text.Json - unable to build without Json framework preference",
            "JsonSwitch",
            DiagnosticSeverity.Error,
            true);

        public void Execute(GeneratorExecutionContext context)
        {
            if(context.SyntaxReceiver is JsonSwitchReceiver switchReceiver)
            {
                var classToUpdate = switchReceiver.ClassToSupercede;
                var hasNewtonsoft =
                    context.Compilation.ReferencedAssemblyNames.Any(ran => ran.Name.Contains("Newtonsoft"));
                var hasSystemTextJson =
                    context.Compilation.ReferencedAssemblyNames.Any(ran => ran.Name.Contains("System.Text.Json"));
                if (hasNewtonsoft)
                {
                    context.ReportDiagnostic(Diagnostic.Create(NewtonsoftExists,Location.None));
                }
                else if (hasSystemTextJson)
                {
                    context.ReportDiagnostic(Diagnostic.Create(SystemTextJsonExists, Location.None));
                }
                else
                {
                    context.ReportDiagnostic(Diagnostic.Create(NeitherFramework, Location.None));
                }
            }
        }
    }

    public class JsonSwitchReceiver:ISyntaxReceiver
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
}
