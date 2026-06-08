using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnusedLocalVariableAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: "FT0005",
            title: "Unused local variable",
            messageFormat: "Local variable '{0}' is declared but never read",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Local variables that are declared but never read are dead code and should be removed.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCodeBlockAction(AnalyzeCodeBlock);
        }

        private static void AnalyzeCodeBlock(CodeBlockAnalysisContext context)
        {
            var semanticModel = context.SemanticModel;
            var root = context.CodeBlock;

            // Collect all local variable declarations within this code block
            var declarations = root
                .DescendantNodes()
                .OfType<VariableDeclaratorSyntax>();

            foreach (var declarator in declarations)
            {
                if (!(semanticModel.GetDeclaredSymbol(declarator) is ILocalSymbol symbol))
                    continue;

                // Skip discards (_)
                if (symbol.Name == "_")
                    continue;

                // Find all references to this symbol within the block
                bool hasRead = HasReadReference(root, symbol, semanticModel);
                if (!hasRead)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        Rule,
                        declarator.GetLocation(),
                        symbol.Name));
                }
            }
        }

        private static bool HasReadReference(
            SyntaxNode root,
            ILocalSymbol symbol,
            SemanticModel semanticModel)
        {
            // Find all identifier names that refer to this symbol
            var identifiers = root
                .DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Where(id => id.Identifier.ValueText == symbol.Name);

            foreach (var identifier in identifiers)
            {
                ISymbol refSymbol = semanticModel.GetSymbolInfo(identifier).Symbol;
                if (!SymbolEqualityComparer.Default.Equals(refSymbol, symbol))
                    continue;

                // A write-only assignment is not a read:
                // e.g. `x = 5;` — the left-hand side of a simple assignment
                if (IsWriteOnlyAssignmentTarget(identifier))
                    continue;

                // Anything else is a read (including compound assignments like +=,
                // which both read and write)
                return true;
            }

            return false;
        }

        private static bool IsWriteOnlyAssignmentTarget(IdentifierNameSyntax identifier)
        {
            // Check if this identifier is the left-hand side of a simple assignment (=)
            // and is not part of a compound assignment (+=, -=, etc.), which also reads.
            if (identifier.Parent is AssignmentExpressionSyntax assignment
                && assignment.Kind() == SyntaxKind.SimpleAssignmentExpression
                && assignment.Left == identifier)
            {
                return true;
            }

            return false;
        }
    }
}
