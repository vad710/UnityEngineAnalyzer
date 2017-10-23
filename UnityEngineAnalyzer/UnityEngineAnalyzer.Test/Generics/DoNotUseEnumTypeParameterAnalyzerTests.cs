using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;
using UnityEngineAnalyzer.Generics;


//using Microsoft.CodeAnalysis.Workspaces;

namespace UnityEngineAnalyzer.Test.GCAlloc
{
    [TestFixture]
    sealed class DoNotUseEnumTypeParameterAnalyzerTests : AnalyzerTestFixture
    {

        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseEnumTypeParameterAnalyzer();

        [Test]
        public void UseEnumTypeParameter()
        {
            var code = @"

class C
{
    public enum MyEnum { One, Two }
    public Dictionary<[|MyEnum|], int> map;
}

";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, null,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.DoNoUseEnumTypeParameter);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }


        [Test]
        public void UseEnumTypeParameter2()
        {
            var code = @"

class C
{
    public enum MyEnum { One, Two }
    public Dictionary<int, Dictionary<[|MyEnum|], int>> map;
}

";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, null,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.DoNoUseEnumTypeParameter);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }



        [Test]
        public void UseEnumTypeParameter3()
        {
            var code = @"

class C
{
    public enum MyEnum { One, Two }
    public Dictionary<int, Dictionary<[|MyEnum|], Dictionary<int, int>>> map;
}

";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, null,
                out document, out span))
            {
                HasDiagnostic(document, span, DiagnosticIDs.DoNoUseEnumTypeParameter);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }


        [Test]
        public void UseNoEnumTypeParameter()
        {
            var code = @"

class C
{
    public enum MyEnum { One, Two }
    public Dictionary<[|int|], int> map;
}

";

            Document document;
            TextSpan span;

            if (TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, null,
                out document, out span))
            {
                NoDiagnostic(document, DiagnosticIDs.DoNoUseEnumTypeParameter);
            }
            else
            {
                Assert.Fail("Could not load unit test code");
            }
        }
    }
}
