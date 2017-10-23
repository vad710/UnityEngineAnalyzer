using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnityEngineAnalyzer.IL2CPP
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnsealedDerivedClassAnalyzer : DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeClassSyntax, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeClassSyntax(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            if (classDeclaration.IsDerived() && !classDeclaration.IsSealed())
            {
                var methods = classDeclaration.Members.OfType<MethodDeclarationSyntax>();

                foreach (var method in methods)
                {

                    if (method.IsOverriden() && !method.IsSealed())
                    {
                        var diagnostic = Diagnostic.Create(SupportedDiagnostics.First(), method.Identifier.GetLocation(),
                            method.Identifier.ToString(), classDeclaration.Identifier.ToString());

                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.UnsealedDerivedClass);
    }
}
