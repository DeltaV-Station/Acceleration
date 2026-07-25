using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DeltaV.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NamingConventionAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [Diagnostics.ClassPrefixRule];

    private static readonly HashSet<string> BaseTypes =
    [
        "Robust.Shared.GameObjects.Component",
        "Robust.Shared.Prototypes.IPrototype",
    ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(compilationContext =>
        {
            var baseTypes = BaseTypes
                .Select(name => compilationContext.Compilation.GetTypeByMetadataName(name))
                .Where(name => name is not null)
                .ToHashSet(SymbolEqualityComparer.Default);

            if (baseTypes.Count == 0)
            {
                return;
            }

            compilationContext.RegisterSymbolAction(
                ctx => AnalyzeSymbol(ctx, baseTypes),
                SymbolKind.NamedType);
        });
    }

    private void AnalyzeSymbol(SymbolAnalysisContext context, HashSet<ISymbol?> baseTypes)
    {
        var symbol = (INamedTypeSymbol)context.Symbol;

        if (!Diagnostics.IsRelevantNamespace(symbol))
        {
            return;
        }

        if (!IsRelevantType(symbol, baseTypes))
        {
            return;
        }

        if (!symbol.Name.StartsWith(Diagnostics.Prefix))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Diagnostics.ClassPrefixRule,
                symbol.Locations[0],
                symbol.Name,
                Diagnostics.Prefix));
        }
    }

    private static bool IsRelevantType(ITypeSymbol symbol, HashSet<ISymbol?> baseTypes)
    {
        var baseType = symbol.BaseType;
        while (baseType is not null)
        {
            if (baseTypes.Contains(baseType))
                return true;

            baseType = baseType.BaseType;
        }

        foreach (var iface in symbol.AllInterfaces)
        {
            if (baseTypes.Contains(iface))
                return true;
        }

        return false;
    }
}
