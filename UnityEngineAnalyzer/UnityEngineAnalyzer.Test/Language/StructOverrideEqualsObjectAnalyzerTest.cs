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
    sealed class StructOverrideEqualsObjectAnalyzerTests : AnalyzerTestFixture
    {

        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new StructOverrideEqualsObjectAnalyzer();


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
                HasDiagnostic(document, span, DiagnosticIDs.StructShouldOverrideEquals);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        [Test]
        public void StructOverrideEqualsObject()
        {
            var code = @"
using UnityEngine;

struct [|S|]
{
    public override bool Equals(object obj) { return false; }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.StructShouldOverrideEquals);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }
    }
}
