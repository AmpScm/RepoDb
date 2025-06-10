using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RepoDb.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DbAttributeFormatAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor EscapedIdentifierRule = new DiagnosticDescriptor(
        "REPDB002",
        "Escaped identifier in attribute",
        "Avoid using escaping in '{0}' attribute: '{1}'",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Escaping in Table, Column and Map attributes is not recommended and may cause portability issues.");

    private static readonly DiagnosticDescriptor TableSchemaFormatRule = new DiagnosticDescriptor(
        "REPDB003",
        "Improper Table attribute format",
        "Avoid schema-qualified table name in [{2}] attribute string. Use Schema property instead: Table(\"{0}\", Schema = \"{1}\").",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Use the 'Schema' property in the Table attribute instead of encoding schema and table in a single string.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(EscapedIdentifierRule, TableSchemaFormatRule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
    }

    private static void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not AttributeSyntax attribute)
            return;

        var semanticModel = context.SemanticModel;
        var symbolInfo = semanticModel.GetSymbolInfo(attribute);

        if (symbolInfo.Symbol is not IMethodSymbol ctorSymbol)
            return;

        var attrType = ctorSymbol.ContainingType;

        var attrName = attrType.Name;
        if (attrName is not ("TableAttribute" or "ColumnAttribute" or "MapAttribute"))
            return;

        var fullAttrName = attrType.ToDisplayString();

        if (fullAttrName is not ("System.ComponentModel.DataAnnotations.Schema.TableAttribute" or "System.ComponentModel.DataAnnotations.Schema.ColumnAttribute" or "RepoDb.Attributes.MapAttribute"))
            return;

        // Analyze argument(s)
        var args = attribute.ArgumentList?.Arguments;
        if (!(args?.Count > 0))
            return;

        var firstArg = args.Value[0];

        var constantValue = semanticModel.GetConstantValue(firstArg.Expression);
        if (!constantValue.HasValue || constantValue.Value is not string strValue)
            return;

        // Warn for square-bracketed identifiers
        if (strValue.Contains('[') || strValue.Contains(']') || strValue.Contains('`') || strValue.Contains('"'))
        {
            var diagnostic = Diagnostic.Create(
                EscapedIdentifierRule,
                firstArg.GetLocation(),
                attrName.Replace("Attribute", ""),
                strValue);
            context.ReportDiagnostic(diagnostic);
        }

        // Specifically for [Table], warn if string contains dot notation for schema
        if (attrName == "TableAttribute" || attrName == "MapAttribute")
        {
            var match = Regex.Match(strValue, @"^\[?(?<schema>\w+)\]?\.\[?(?<table>\w+)\]?$");
            if (match.Success)
            {
                var schema = match.Groups["schema"].Value;
                var table = match.Groups["table"].Value;
                var diagnostic = Diagnostic.Create(
                    TableSchemaFormatRule,
                    firstArg.GetLocation(),
                    table,
                    schema,
                    attrName.Replace("Attribute", ""));
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
