using Microsoft.CodeAnalysis;

namespace DeltaV.Analyzers;

public static class Diagnostics
{
    public const string Namespace = "_DVA";
    public const string Prefix = "DV";
    public const string PrototypePrefix = "dv";

    public static readonly DiagnosticDescriptor ClassPrefixRule = new(
        "DV001",
        $"Type in Delta-V code must have required prefix {Prefix}",
        "Type '{0}' in Delta-V code must start with '{1}'",
        "Naming",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor MissingExplicitPrototypeName = new(
        "DV002",
        $"Prototype in Delta-V code must have an explicit prototype name starting with '{PrototypePrefix}'",
        $"Prototype '{{0}}' in Delta-V code is missing an explicit prototype name starting with '{PrototypePrefix}'",
        "Naming",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor WrongExplicitPrototypeName = new(
        "DV003",
        $"Prototype in Delta-V code must have an explicit prototype name starting with '{PrototypePrefix}'",
        $"Prototype '{{0}}' in Delta-V code has an incorrect explicit prototype name {{1}} that should start with '{PrototypePrefix}'",
        "Naming",
        DiagnosticSeverity.Error,
        true);

    public static bool IsRelevantNamespace(ISymbol symbol)
    {
        var parent = symbol.ContainingNamespace;
        return parent?.ToDisplayString().Contains(Diagnostics.Namespace) == true;
    }

}
