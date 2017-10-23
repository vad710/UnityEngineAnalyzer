using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace UnityEngineAnalyzer.Delegates
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ShouldCacheDelegateAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.ShouldCacheDelegate);
        
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeAssignmentNode, SyntaxKind.AddAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAssignmentNode, SyntaxKind.SubtractAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeInvocationNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeAssignmentNode(SyntaxNodeAnalysisContext context)
        {
            var checkSyntax = context.Node;
            
            foreach (var oneIdSyntax in checkSyntax.DescendantNodes().OfType<IdentifierNameSyntax>())
            {
                var oneIdSymbol = context.SemanticModel.GetSymbolInfo(oneIdSyntax);
                if (oneIdSymbol.Symbol != null)
                {
                    if (oneIdSymbol.Symbol is IMethodSymbol)
                    {
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.ShouldCacheDelegate,
                            oneIdSyntax.GetLocation(), oneIdSyntax.ToString());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private static void AnalyzeInvocationNode(SyntaxNodeAnalysisContext context)
        {
            var checkSyntax = context.Node as InvocationExpressionSyntax;
            if(null == checkSyntax)
            {
                return;
            }

            var argumentListSyntax = checkSyntax.ChildNodes().OfType<ArgumentListSyntax>();
            foreach (var oneArgListSyntax in argumentListSyntax)
            {
                foreach(var oneArgSyntax in oneArgListSyntax.ChildNodes().OfType<ArgumentSyntax>())
                {
                    foreach (var oneIdSyntax in oneArgSyntax.ChildNodes().OfType<IdentifierNameSyntax>())
                    {
                        var oneIdSymbol = context.SemanticModel.GetSymbolInfo(oneIdSyntax);
                        if (oneIdSymbol.Symbol != null)
                        {
                            if (oneIdSymbol.Symbol is IMethodSymbol)
                            {
                                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.ShouldCacheDelegate,
                                    oneIdSyntax.GetLocation(), oneIdSyntax.ToString());
                                context.ReportDiagnostic(diagnostic);
                            }
                        }
                    }
                }
            }
        }
    }
}
