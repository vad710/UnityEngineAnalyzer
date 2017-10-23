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
    sealed class StructImplementationAnalyzerTests : AnalyzerTestFixture
    {

        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new StructImplementAnalyzer();

        [Test]
        public void StructDoNotImplementIEquatable()
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
                HasDiagnostic(document, span, DiagnosticIDs.StructShouldImplementIEquatable);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }



        [Test]
        public void StructDoImplementIEquatable()
        {
            var code = @"
using UnityEngine;
using System;

struct [|S|] : IEquatable<S>
{
    public bool Equals(S other) { return false; }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.StructShouldImplementIEquatable);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }


        [Test]
        public void StructDoNotImplementCorrectIEquatable()
        {
            var code = @"
using UnityEngine;
using System;


struct [|S|] : IEquatable<int>
{
    public bool Equals(int other) { return false; }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.StructShouldImplementIEquatable);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }
        
    }
}
