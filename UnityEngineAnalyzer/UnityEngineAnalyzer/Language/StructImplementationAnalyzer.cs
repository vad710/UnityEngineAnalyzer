using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnityEngineAnalyzer.Language
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StructImplementAnalyzer : DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeClassSyntax, SyntaxKind.StructDeclaration);
        }

        private void AnalyzeClassSyntax(SyntaxNodeAnalysisContext context)
        {
            CheckStructShouldImplementIEquatable(context);
        }


        private void CheckStructShouldImplementIEquatable(SyntaxNodeAnalysisContext context)
        {
            var declSyntax = (StructDeclarationSyntax)context.Node;
            var declSymbol = context.SemanticModel.GetDeclaredSymbol(declSyntax) as INamedTypeSymbol;

            bool foundIEquatableOfStruct = false;

            if (declSyntax.BaseList != null)
            {

                var baseList = declSyntax.BaseList.Types;
                var baseTypesSyntax = baseList.OfType<SimpleBaseTypeSyntax>();


                foreach (var oneBaseTypeSyntax in baseTypesSyntax)
                {
                    var baseTypeName = oneBaseTypeSyntax.ChildNodes();
                    foreach (var oneName in baseTypeName)
                    {
                        if (oneName is GenericNameSyntax)
                        {
                            var genericSymbolInfo = context.SemanticModel.GetSymbolInfo(oneName);
                            var genericSymbol = genericSymbolInfo.Symbol as INamedTypeSymbol;
                            if (genericSymbol != null && genericSymbol.ContainingNamespace.Name == "System" &&
                                genericSymbol.Name == "IEquatable")
                            {
                                foreach (var oneParameterType in genericSymbol.TypeArguments)
                                {
                                    if (oneParameterType == declSymbol)
                                    {
                                        foundIEquatableOfStruct = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!foundIEquatableOfStruct)
            {

                var diagnostic = Diagnostic.Create(SupportedDiagnostics.First(), declSyntax.Identifier.GetLocation(),
                    declSyntax.Identifier.ToString());

                context.ReportDiagnostic(diagnostic);
            }
        }
        

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
            ImmutableArray.Create(
                DiagnosticDescriptors.StructShouldImplementIEquatable
                );
    }
}
