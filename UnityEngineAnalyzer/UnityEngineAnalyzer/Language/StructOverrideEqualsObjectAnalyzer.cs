using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnityEngineAnalyzer.Language
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StructOverrideEqualsObjectAnalyzer : DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeClassSyntax, SyntaxKind.StructDeclaration);
        }

        private void AnalyzeClassSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckStructShouldOverride(context);
        }

        
        private void CheckStructShouldOverride(SyntaxNodeAnalysisContext context)
        {
            var declSyntax = (StructDeclarationSyntax)context.Node;

            bool foundOverrideEquals = false;

            foreach (var oneChild in declSyntax.ChildNodes())
            {
                if(oneChild is MethodDeclarationSyntax)
                {
                    var oneMethodSymbol = context.SemanticModel.GetDeclaredSymbol(oneChild) as IMethodSymbol;
                    if(oneMethodSymbol.IsOverride)
                    {
                        if(oneMethodSymbol.Parameters != null && oneMethodSymbol.Parameters.Length == 1 &&
                            oneMethodSymbol.Name == "Equals")
                        {
                            var oneParamType = oneMethodSymbol.Parameters[0].Type;
                            if (oneParamType.ContainingNamespace.Name == "System" && oneParamType.Name.ToLower() == "object")
                            {
                                foundOverrideEquals = true;
                                break;
                            }
                        }
                        
                    }
                }
            }


            if(!foundOverrideEquals)
            {
                var diagnostic = Diagnostic.Create(SupportedDiagnostics[0], declSyntax.Identifier.GetLocation(),
                    declSyntax.Identifier.ToString());

                context.ReportDiagnostic(diagnostic);
            }
        }
        

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
            ImmutableArray.Create(
                DiagnosticDescriptors.StructShouldOverrideEquals
                );
    }
}
