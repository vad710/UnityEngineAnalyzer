using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.Language;


//using Microsoft.CodeAnalysis.Workspaces;

namespace UnityEngineAnalyzer.Test.Language
{
    [TestFixture]
    sealed class StructOverrideGetHashCodeTests : AnalyzerTestFixture
    {

        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new StructOverrideGetHashCodeAnalyzer();
        
        
        [Test]
        public void StructDoNotOverrideAnything()
        {
            var code = @"
using UnityEngine;

struct [|S|]
{
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.StructShouldOverrideGetHashCode);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }
        

        [Test]
        public void StructOverridGetHashCode()
        {
            var code = @"
using UnityEngine;

struct [|S|]
{
    public override int GetHashCode() { return 0; }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.StructShouldOverrideGetHashCode);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }
        
    }
}
