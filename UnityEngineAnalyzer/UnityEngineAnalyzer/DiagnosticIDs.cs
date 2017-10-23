
namespace UnityEngineAnalyzer
{
    public static class DiagnosticIDs
    {
        public const string DoNotUseOnGUI = "UEA0001";
        public const string DoNotUseStringMethods = "UEA0002";
        public const string EmptyMonoBehaviourMethod = "UEA0003";
        public const string UseCompareTag = "UEA0004";
        public const string DoNotUseFindMethodsInUpdate = "UEA0005";
        public const string DoNotUseCoroutines = "UEA0006";
        public const string DoNotUseForEachInUpdate = "UEA0007";
        public const string UnsealedDerivedClass = "UEA0008";
        public const string InvokeFunctionMissing = "UEA0009";
        public const string DoNotUseStateNameInAnimator = "UEA0010";
        public const string DoNotUseStringPropertyNamesInMaterial = "UEA0011";
        public const string CameraMainIsSlow = "UEA0012";
        public const string PhysicsUseNonAllocMethods = "UEA0013";
        public const string DoNotGCAllocInUpdate = "UEA0014";
      

        //NOTES: These should probably be on their own analyzer - as they are not specific to Unity
        public const string DoNotUseRemoting = "AOT0001";
        public const string DoNotUseReflectionEmit = "AOT0002";
        public const string TypeGetType = "AOT0003";
        public const string StructShouldImplementIEquatable = "AOT0004";
        public const string StructShouldOverrideEquals = "AOT0005";
        public const string StructShouldOverrideGetHashCode = "AOT0006";
        public const string DoNotBoxWhenInvoke = "AOT0007";
        public const string DoNoUseEnumTypeParameter = "AOT0008";
        public const string ShouldCacheDelegate = "AOT0009";
    }
}
