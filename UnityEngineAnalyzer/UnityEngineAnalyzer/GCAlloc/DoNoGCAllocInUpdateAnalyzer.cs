using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace UnityEngineAnalyzer.GCAlloc
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    /// DoNotGCAllocInUpdateAnalyzer currently is conservative,
    /// which means, if the ObjectCreationExpressionSyntax is inside a branch (if/switch),
    /// then this analyzer will NOT report diagnose.
    public sealed class DoNotGCAllocInUpdateAnalyzer : DiagnosticAnalyzer
    {

        private Dictionary<ExpressionSyntax,ExpressionSyntax> _indirectCallers;


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.DoNotGCAllocnInUpdate,
                    DiagnosticDescriptors.DoNotGCAllocnInUpdateRecursive);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeClassSyntax, SyntaxKind.ClassDeclaration);
        }

        public void AnalyzeClassSyntax(SyntaxNodeAnalysisContext context)
        {
            var monoBehaviourInfo = new MonoBehaviourInfo(context);

            var searched = new Dictionary<ISymbol, bool>();
            _indirectCallers = new Dictionary<ExpressionSyntax, ExpressionSyntax>();

            monoBehaviourInfo.ForEachUpdateMethod((updateMethod) =>
            {
                //Debug.WriteLine("Found an update call! " + updateMethod);

                var results = SearchForTargetExpression(context, updateMethod, searched, true);

                foreach (var oneResult in results)
                {
                    if (!_indirectCallers.ContainsKey(oneResult))
                    {
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotGCAllocnInUpdate,
                            oneResult.GetLocation(), oneResult, monoBehaviourInfo.ClassName, updateMethod.Identifier);
                        context.ReportDiagnostic(diagnostic);
                    }
                    else
                    {
                        var endPoint = _indirectCallers[oneResult];

                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DoNotGCAllocnInUpdateRecursive,
                            oneResult.GetLocation(), monoBehaviourInfo.ClassName, updateMethod.Identifier, oneResult, endPoint);
                        context.ReportDiagnostic(diagnostic);
                    }

                }
            });
        }

        //TODO: Try to simplify this method - it's very hard to follow
        private IEnumerable<ExpressionSyntax> SearchForTargetExpression(SyntaxNodeAnalysisContext context,
            MethodDeclarationSyntax method, IDictionary<ISymbol, bool> searchedSymbol, bool isRoot)
        {
            var targetExps = method.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
            foreach (var oneTargetExp in targetExps)
            {
                /*
                var oneTargetIdentifierExps = oneTargetExp.DescendantNodes().OfType<IdentifierNameSyntax>();
                foreach(var oneTargetIdentifierExp in oneTargetIdentifierExps)
                {
                }
                */

                bool isContainedByBranch = false;

                SyntaxNode parent = oneTargetExp;
                while (parent != method)
                {
                    if (parent is IfStatementSyntax || parent is SwitchStatementSyntax)
                    {
                        isContainedByBranch = true;
                        break;
                    }

                    parent = parent.Parent;
                }

                if (isContainedByBranch)
                {
                    continue;
                }


                SymbolInfo oneSymbolInfo;
                if (!context.TryGetSymbolInfo(oneTargetExp, out oneSymbolInfo))
                {
                    continue;
                }

                var targetSymbol = oneSymbolInfo.Symbol as IMethodSymbol;
                if (targetSymbol != null)
                {
                    if (searchedSymbol.ContainsKey(targetSymbol))
                    {
                        if (searchedSymbol[targetSymbol])
                        {
                            yield return (ExpressionSyntax)oneTargetExp;
                        }
                    }
                    else
                    {
                        if (targetSymbol.ReceiverType.IsReferenceType)
                        {
                            searchedSymbol.Add(targetSymbol, true);
                            yield return oneTargetExp;
                        }
                    }
                }
            }


            var invocationExps = method.DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var oneInvocationExp in invocationExps)
            {
                SymbolInfo oneSymbolInfo;
                if (!context.TryGetSymbolInfo(oneInvocationExp, out oneSymbolInfo))
                {
                    continue;
                }


                bool isContainedByBranch = false;
                SyntaxNode parent = oneInvocationExp;
                while (parent != method)
                {
                    if (parent is IfStatementSyntax || parent is SwitchStatementSyntax)
                    {
                        isContainedByBranch = true;
                        break;
                    }

                    parent = parent.Parent;
                }
                if (isContainedByBranch)
                {
                    continue;
                }

                var methodSymbol = oneSymbolInfo.Symbol as IMethodSymbol;

                if (methodSymbol != null)
                {
                    if (searchedSymbol.ContainsKey(methodSymbol))
                    {
                        if (searchedSymbol[methodSymbol])
                        {
                            yield return oneInvocationExp;
                        }
                    }
                    else
                    {
                        var methodDeclarations = methodSymbol.DeclaringSyntaxReferences;
                        searchedSymbol.Add(methodSymbol, false); //let's assume there won't be any calls

                        foreach (var methodDeclaration in methodDeclarations)
                        {
                            var theMethodSyntax = methodDeclaration.GetSyntax() as MethodDeclarationSyntax;

                            if (theMethodSyntax != null)
                            {
                                var childResults = SearchForTargetExpression(context, theMethodSyntax, searchedSymbol, false);

                                if (childResults != null && childResults.Any())
                                {
                                    searchedSymbol[methodSymbol] = true; //update the searched dictionary with new info

                                    if (isRoot)
                                    {
                                        _indirectCallers.Add(oneInvocationExp, childResults.First());
                                    }

                                    yield return oneInvocationExp;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
