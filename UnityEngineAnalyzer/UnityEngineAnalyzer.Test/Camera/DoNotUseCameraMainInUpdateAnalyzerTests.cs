using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.FindMethodsInUpdate;


//using Microsoft.CodeAnalysis.Workspaces;

namespace UnityEngineAnalyzer.Test.FindMethodsInUpdate
{
    [TestFixture]
    sealed class DoNotUseCameraMainInUpdateAnalyzerTests : AnalyzerTestFixture
    {

        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseCameraMainInUpdateAnalyzer();

        [Test]
        public void CameraMainInUpdate()
        {
            var code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Update()
    {
        Camera main = [|Camera.main|];

        //var result = GameObject.Find(""param"");
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.DoNotUseCameraMainInUpdate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }


        [Test]
        public void CameraMainInUpdateRecursive()
        {
            var code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Update()
    {
        [|MyMethod()|];
        //var result = GameObject.Find(""param"");
    }

    void MyMethod()
    {
        Camera main = Camera.main;
    }
}";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.DoNotUseCameraMainInUpdate);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }
        
    }
}
