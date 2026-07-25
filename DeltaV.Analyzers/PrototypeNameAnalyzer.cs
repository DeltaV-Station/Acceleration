using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DeltaV.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PrototypeNameAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [Diagnostics.MissingExplicitPrototypeName, Diagnostics.WrongExplicitPrototypeName];

    private const string AttributeType = "Robust.Shared.Prototypes.PrototypeAttribute";
    private const string InterfaceType = "Robust.Shared.Prototypes.IPrototype";

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compilationContext =>
        {
            var attributeSymbol = compilationContext.Compilation
                .GetTypeByMetadataName(AttributeType);

            var ifaceSymbol = compilationContext.Compilation
                .GetTypeByMetadataName(InterfaceType);

            if (attributeSymbol is null || ifaceSymbol is null)
                return;

            compilationContext.RegisterSymbolAction(
                ctx => AnalyzeSymbol(ctx, attributeSymbol, ifaceSymbol),
                SymbolKind.NamedType);
        });
    }

    private void AnalyzeSymbol(SymbolAnalysisContext context, INamedTypeSymbol prototypeAttribute, INamedTypeSymbol prototypeIface)
    {
        var symbol = (INamedTypeSymbol)context.Symbol;

        if (!Diagnostics.IsRelevantNamespace(symbol))
        {
            return;
        }

        if (!IsRelevantType(symbol, prototypeIface))
        {
            return;
        }

        foreach (var attribute in symbol.GetAttributes())
        {
            if (!SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, prototypeAttribute))
                continue;

            var attributeLocation = GetReportLocation(attribute) ?? symbol.Locations.FirstOrDefault();

            if (attribute.ConstructorArguments.Length != 2)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.MissingExplicitPrototypeName,
                    attributeLocation,
                    symbol.Name));
                continue;
            }

            var arg = attribute.ConstructorArguments[0];
            if (arg.Kind != TypedConstantKind.Primitive || arg.Value is not string idValue)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.MissingExplicitPrototypeName,
                    attributeLocation,
                    symbol.Name));

                continue;
            }

            if (idValue.StartsWith(Diagnostics.PrototypePrefix))
                continue;

            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.WrongExplicitPrototypeName,
                attributeLocation,
                symbol.Name,
                idValue));
        }
    }

    private static bool IsRelevantType(ITypeSymbol symbol, INamedTypeSymbol expectedIface)
    {
        foreach (var iface in symbol.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(iface, expectedIface))
                return true;
        }

        return false;
    }

    private static Location? GetReportLocation(AttributeData attributeData)
    {
        var syntaxRef = attributeData.ApplicationSyntaxReference;
        if (syntaxRef is null)
            return null;

        if (syntaxRef.GetSyntax() is AttributeSyntax attributeSyntax
            && attributeSyntax.ArgumentList?.Arguments.Count > 0)
        {
            return attributeSyntax.ArgumentList.Arguments[0].GetLocation();
        }

        return syntaxRef.GetSyntax().GetLocation();
    }
}
