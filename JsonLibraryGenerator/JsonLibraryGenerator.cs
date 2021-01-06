using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace JsonLibraryGenerator
{
    [Generator]
    public class JsonLibraryGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new JsonSwitchReceiver());
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
            "Found no references to either Newtonsoft or System.Text.Json - unable to build without Json framework preference",
            "JsonSwitch",
            DiagnosticSeverity.Error,
            true);

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not JsonSwitchReceiver switchReceiver ||
                switchReceiver.ClassToSupercede == null)
            {
                return;
            }

            var hasNewtonsoft =
                context.Compilation.ReferencedAssemblyNames.Any(ran => ran.Name.Contains("Newtonsoft"));
            var hasSystemTextJson =
                context.Compilation.ReferencedAssemblyNames.Any(ran => ran.Name.Contains("System.Text.Json"));
            if (hasNewtonsoft)
            {
                context.ReportDiagnostic(Diagnostic.Create(NewtonsoftExists, Location.None));
            }
            else if (hasSystemTextJson)
            {
                context.ReportDiagnostic(Diagnostic.Create(SystemTextJsonExists, Location.None));
            }
            else
            {
                context.ReportDiagnostic(Diagnostic.Create(NeitherFramework, Location.None));
            }

            var classToUpdate = switchReceiver.ClassToSupercede;
            var classGenerator = hasNewtonsoft ? (JsonClassGenerator)new NewtonsoftGenerator() : new SystemTextJsonGenerator();
            var overrideClass = classGenerator.Generate(classToUpdate);

            context.AddSource(Guid.NewGuid().ToString("N"), overrideClass);
        }
    }
}
