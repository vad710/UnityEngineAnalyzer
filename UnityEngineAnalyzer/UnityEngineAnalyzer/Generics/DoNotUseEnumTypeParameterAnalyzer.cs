using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace UnityEngineAnalyzer.Generics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseEnumTypeParameterAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseEnumTypeParameter);
        
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.VariableDeclaration);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var targetSyntax = context.Node as VariableDeclarationSyntax;
            if (targetSyntax == null)
            {
                return;
            }

            System.Collections.Generic.HashSet<IdentifierNameSyntax> checkedIdSyntax = new System.Collections.Generic.HashSet<IdentifierNameSyntax>();

            var typeArgumentLists = targetSyntax.DescendantNodes().OfType<TypeArgumentListSyntax>();
            foreach(var oneTypeArgSyntax in typeArgumentLists)
            {
                foreach(var oneIdSyntax in oneTypeArgSyntax.DescendantNodes().OfType<IdentifierNameSyntax>())
                {
                    if(!checkedIdSyntax.Contains(oneIdSyntax))
                    {
                        checkedIdSyntax.Add(oneIdSyntax);

                        var oneIdSymbolInfo = context.SemanticModel.GetSymbolInfo(oneIdSyntax);
                        var oneIdSymbol = oneIdSymbolInfo.Symbol as INamedTypeSymbol;
                        if (oneIdSymbol.BaseType.Name == "Enum" &&
                            oneIdSymbol.BaseType.ContainingNamespace.Name == "System")
                        {
                            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseEnumTypeParameter,
                                oneIdSyntax.GetLocation(), oneIdSyntax.ToString());
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                }
            }

        }
    }
}
