using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.GCAlloc;


//using Microsoft.CodeAnalysis.Workspaces;

namespace UnityEngineAnalyzer.Test.GCAlloc
{
    [TestFixture]
    sealed class DoNotBoxWhenInvokeAnalyzerTests : AnalyzerTestFixture
    {

        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotBoxWhenInvokeAnalyzer();

        [Test]
        public void BoxWhenInvokeWithLiteral()
        {
            var code = @"

class C
{
    private void Method(object p1, int p2)
    {
    }

    private void Caller()
    {
        Method([|234|], 2);
    }
}

";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, null,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.DoNotBoxWhenInvoke);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }


        [Test]
        public void BoxWhenInvokeWithIdentifier()
        {
            var code = @"

class C
{
    private void Method(object p1, int p2)
    {
    }

    private void Caller()
    {
        int arg = 234;
        Method([|arg|], 2);
    }
}

";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, null,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.DoNotBoxWhenInvoke);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

        [Test]
        public void NoBoxWhenInvokeWithLiteral()
        {
            var code = @"

class C
{
    private void Method(int p1, int p2)
    {
    }

    private void Caller()
    {
        Method([|234|], 2);
    }
}

";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, null,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.DoNotBoxWhenInvoke);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }



        [Test]
        public void NoBoxWhenInvokeWithIdentifier()
        {
            var code = @"

class C
{
    private void Method(int p1, int p2)
    {
    }

    private void Caller()
    {
        int arg = 234;
        Method([|arg|], 2);
    }
}

";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, null,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.DoNotBoxWhenInvoke);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }


        [Test]
        public void BoxIgnoredWhenConditionalInvokeWithLiteral()
        {
            var code = @"

using System.Diagnostics;

class C
{
    [Conditional(""SOME_CONDITION""), Conditional(""SOME_CONDITION2"")]
    private void Method(object p1, int p2)
    {
    }

    private void Caller()
    {
        Method([|234|], 2);
    }
}

";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, null,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.DoNotBoxWhenInvoke);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }

    }
}
