using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace UnityEngineAnalyzer.GCAlloc
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotBoxWhenInvokeAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotBoxWhenInvoke);
        
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocation = context.Node as InvocationExpressionSyntax;
            if (invocation == null)
            {
                return;
            }
            
            if (invocation.ArgumentList == null || invocation.ArgumentList.Arguments == null || invocation.ArgumentList.Arguments.Count == 0)
            {
                return;
            }

            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
            var methodSymbol = symbolInfo.Symbol as IMethodSymbol;

            bool isConditional = false;
            foreach(var oneAttribute in methodSymbol.GetAttributes())
            {
                if(oneAttribute.AttributeClass.Name == "ConditionalAttribute" && oneAttribute.AttributeClass.ContainingNamespace.Name == "Diagnostics")
                {
                    isConditional = true;
                    break;
                }

            }

            if(isConditional)
            {
                return;
            }


            if(methodSymbol.Parameters == null || methodSymbol.Parameters.Length == 0)
            {
                return;
            }

            for(int i = 0; i < methodSymbol.Parameters.Length && i < invocation.ArgumentList.Arguments.Count; ++i)
            {
                var oneArgSyntax = invocation.ArgumentList.Arguments[i];
                if(oneArgSyntax == null)
                {
                    continue;
                }
                var oneArgType = context.SemanticModel.GetTypeInfo(oneArgSyntax.Expression);
                if(oneArgType.Type == null)
                {
                    continue;
                }

                var oneParamType = methodSymbol.Parameters[i];

                if(oneParamType.Type == null)
                {
                    return;
                }

                if(oneArgType.Type.IsValueType && oneParamType.Type.IsReferenceType)
                {
                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotBoxWhenInvoke, oneArgSyntax.Expression.GetLocation(),
                        methodSymbol.ContainingType.Name, methodSymbol.Name, oneParamType.Name, oneParamType.Type.ToString(),
                        oneArgSyntax.Expression.ToString(), oneArgType.Type.ToString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
