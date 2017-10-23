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
    sealed class DoNotGCAllocInUpdateAnalyzerTests : AnalyzerTestFixture
    {

        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotGCAllocInUpdateAnalyzer();

        [Test]
        public void GCAllocValueTypeInUpdate()
        {
            var code = @"
using UnityEngine;

struct S {}

class C : MonoBehaviour
{
    void Update()
    {
        S s = [|new S()|];

        //var result = GameObject.Find(""param"");
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.DoNotGCAllocInUpdate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }


        [Test]
        public void GCAllocInUpdate()
        {
            var code = @"
using UnityEngine;

class B {}

class C : MonoBehaviour
{
    void Update()
    {
        B b = [|new B()|];

        //var result = GameObject.Find(""param"");
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.DoNotGCAllocInUpdate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }



        [Test]
        public void GCAllocInBranchInUpdate()
        {
            var code = @"
using UnityEngine;

class B {}

class C : MonoBehaviour
{
    void Update()
    {
        B b;
        if(b == null)
        {
            b = [|new B()|];
        };

        //var result = GameObject.Find(""param"");
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.DoNotGCAllocInUpdate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }


        [Test]
        public void GCAllocInUpdateRecursive()
        {
            var code = @"
using UnityEngine;

class B {}

class C : MonoBehaviour
{
    void Update()
    {
        [|MyMethod()|];
        //var result = GameObject.Find(""param"");
    }

    void MyMethod()
    {
        B b = new B();
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.DoNotGCAllocInUpdate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }



        [Test]
        public void GCAllocInUpdateRecursiveWithBranch()
        {
            var code = @"
using UnityEngine;

class B {}

class C : MonoBehaviour
{
    void Update()
    {
        if(true)
        {
            [|MyMethod()|];
        }
        //var result = GameObject.Find(""param"");
    }

    void MyMethod()
    {
        B b = new B();
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.DoNotGCAllocInUpdate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }


        [Test]
        public void GCAllocInUpdateRecursiveWithBranch2()
        {
            var code = @"
using UnityEngine;

class B {}

class C : MonoBehaviour
{
    void Update()
    {
        [|MyMethod()|];
        //var result = GameObject.Find(""param"");
    }

    void MyMethod()
    {
        if(true)
        {
            B b = new B();
        }
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.DoNotGCAllocInUpdate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }


        [Test]
        public void GCAllocValueTypeInUpdateRecursive()
        {
            var code = @"
using UnityEngine;

struct S {}

class C : MonoBehaviour
{
    void Update()
    {
        [|MyMethod()|];
        //var result = GameObject.Find(""param"");
    }

    void MyMethod()
    {
        S s = new S();
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.DoNotGCAllocInUpdate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }
    }
}
