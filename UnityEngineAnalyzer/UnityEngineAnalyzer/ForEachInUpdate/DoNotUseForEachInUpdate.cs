using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnityEngineAnalyzer.ForEachInUpdate
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseForEachInUpdate : DiagnosticAnalyzer
    {

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseForEachInUpdate);
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeClassSyntax, SyntaxKind.ClassDeclaration);
        }

        public static void AnalyzeClassSyntax(SyntaxNodeAnalysisContext context)
        {
            var monoBehaviourInfo = new MonoBehaviourInfo(context);

            

            var searched = new Dictionary<IMethodSymbol, bool>();
            monoBehaviourInfo.ForEachUpdateMethod((updateMethod) =>
            {
                var forEachStatements = SearchForForEach(context, updateMethod, searched);

                foreach (var forEachStatement in forEachStatements)
                {
                    Debug.WriteLine("Found a bad call! " + forEachStatement);

                    var location = forEachStatement.ForEachKeyword.GetLocation();
                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotUseForEachInUpdate, location, monoBehaviourInfo.ClassName ,updateMethod.Identifier);
                    context.ReportDiagnostic(diagnostic);
                }
            });
        }


        private static IEnumerable<ForEachStatementSyntax> SearchForForEach(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method, IDictionary<IMethodSymbol, bool> searched)
        {
            var invocations = method.DescendantNodes().OfType<ForEachStatementSyntax>();

            foreach (var invocation in invocations)
            {
                yield return invocation;
            }

            //TODO: Keep Searching recurively to other methods...
        }
    }
}
