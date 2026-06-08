using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnreachableCodeAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: "FT0004",
            title: "Unreachable code detected",
            messageFormat: "This statement is unreachable and will never execute",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Code appearing after a return, throw, break, or continue statement can never be executed.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeBlock, SyntaxKind.Block);
        }

        private static void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;
            var statements = block.Statements;

            bool foundTerminator = false;
            foreach (var statement in statements)
            {
                if (foundTerminator)
                {
                    // Everything after the first terminator is unreachable
                    context.ReportDiagnostic(Diagnostic.Create(Rule, statement.GetLocation()));
                }

                if (IsUnconditionalTerminator(statement))
                    foundTerminator = true;
            }
        }

        private static bool IsUnconditionalTerminator(StatementSyntax statement)
        {
            switch (statement)
            {
                case ReturnStatementSyntax _:
                case ThrowStatementSyntax _:
                case BreakStatementSyntax _:
                case ContinueStatementSyntax _:
                case GotoStatementSyntax _:
                    return true;

                default:
                    return false;
            }
        }
    }
}
