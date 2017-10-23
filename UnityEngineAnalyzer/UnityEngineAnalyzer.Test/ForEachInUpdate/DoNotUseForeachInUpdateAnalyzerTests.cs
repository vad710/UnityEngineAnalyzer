using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.FindMethodsInUpdate;
using UnityEngineAnalyzer.ForEachInUpdate;

//using Microsoft.CodeAnalysis.Workspaces;

namespace UnityEngineAnalyzer.Test.ForEachInUpdate
{
[TestFixture]
sealed class DoNotUseForeachInUpdateAnalyzerTests : AnalyzerTestFixture
{

    protected override string LanguageName => LanguageNames.CSharp;
    protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseForEachInUpdate();

    [Test]
    public void ForEachInUpdate()
    {
        var code = @"
using UnityEngine;

class C : MonoBehaviour
{
void Update()
{
        var colors = new[] {""red"", ""white"", ""blue""};
        var result = string.Empty;
        [|foreach|] (var color in colors)
        {
            result += color;
        }
    }
}";



        Document document;
        TextSpan span;

        if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
            out document, out span))
        {
            HasDiagnostic(document, span, DiagnosticIDs.DoNotUseForEachInUpdate);
        }
        else
        {
            Assert.Fail("Could not load unit test code");
        }
    }
}
}