using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.IL2CPP;


//using Microsoft.CodeAnalysis.Workspaces;

namespace UnityEngineAnalyzer.Test.GCAlloc
{
    [TestFixture]
    sealed class UnsealedDerivedClassAnalyzerTests : AnalyzerTestFixture
    {

        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new UnsealedDerivedClassAnalyzer();

        [Test]
        public void UnsealedOverrideMethod()
        {
            var code = @"

class C
{
    protected virtual void Method()
    {
    }
}


class D : C
{
    protected override void [|Method|]()
    {
    }
}

";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, null,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.UnsealedDerivedClass);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        
    }
}
