using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Resources;
using UnityEngineAnalyzer.Animator;
using UnityEngineAnalyzer.AOT;
using UnityEngineAnalyzer.Camera;
using UnityEngineAnalyzer.CompareTag;
using UnityEngineAnalyzer.Coroutines;
using UnityEngineAnalyzer.EmptyMonoBehaviourMethods;
using UnityEngineAnalyzer.FindMethodsInUpdate;
using UnityEngineAnalyzer.ForEachInUpdate;
using UnityEngineAnalyzer.IL2CPP;
using UnityEngineAnalyzer.Material;
using UnityEngineAnalyzer.OnGUI;
using UnityEngineAnalyzer.Physics;
using UnityEngineAnalyzer.StringMethods;
using UnityEngineAnalyzer.Language;
using UnityEngineAnalyzer.GCAlloc;
using UnityEngineAnalyzer.Generics;
using UnityEngineAnalyzer.Delegates;

namespace UnityEngineAnalyzer
{
    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor DoNotUseOnGUI;
        public static readonly DiagnosticDescriptor DoNotUseStringMethods;
        public static readonly DiagnosticDescriptor DoNotUseCoroutines;
        public static readonly DiagnosticDescriptor EmptyMonoBehaviourMethod;
        public static readonly DiagnosticDescriptor UseCompareTag;
        public static readonly DiagnosticDescriptor DoNotUseFindMethodsInUpdate;
        public static readonly DiagnosticDescriptor DoNotUseFindMethodsInUpdateRecursive;
        public static readonly DiagnosticDescriptor DoNotUseRemoting;
        public static readonly DiagnosticDescriptor DoNotUseReflectionEmit;
        public static readonly DiagnosticDescriptor TypeGetType;
        public static readonly DiagnosticDescriptor DoNotUseForEachInUpdate;
        public static readonly DiagnosticDescriptor UnsealedDerivedClass;
        public static readonly DiagnosticDescriptor InvokeFunctionMissing;
        public static readonly DiagnosticDescriptor DoNotUseStateName;
        public static readonly DiagnosticDescriptor DoNotUseStringPropertyNames;
        public static readonly DiagnosticDescriptor UseNonAllocMethods;
        public static readonly DiagnosticDescriptor CameraMainIsSlow;
        public static readonly DiagnosticDescriptor DoNotGCAllocnInUpdate;
        public static readonly DiagnosticDescriptor DoNotGCAllocnInUpdateRecursive;
        public static readonly DiagnosticDescriptor DoNotBoxWhenInvoke;
        public static readonly DiagnosticDescriptor StructShouldImplementIEquatable;
        public static readonly DiagnosticDescriptor StructShouldOverrideEquals;
        public static readonly DiagnosticDescriptor StructShouldOverrideGetHashCode;
        public static readonly DiagnosticDescriptor DoNotUseEnumTypeParameter;
        public static readonly DiagnosticDescriptor ShouldCacheDelegate;

        static DiagnosticDescriptors()
        {
            //** UNITY **
            
            //GC
            DoNotUseOnGUI = CreateDiagnosticDescriptor<DoNotUseOnGUIResources>(DiagnosticIDs.DoNotUseOnGUI, DiagnosticCategories.GC, DiagnosticSeverity.Info);
            DoNotUseStringMethods = CreateDiagnosticDescriptor<DoNotUseStringMethodsResources>(DiagnosticIDs.DoNotUseStringMethods, DiagnosticCategories.GC, DiagnosticSeverity.Info);
            DoNotUseCoroutines = CreateDiagnosticDescriptor<DoNotUseCoroutinesResources>(DiagnosticIDs.DoNotUseCoroutines, DiagnosticCategories.GC, DiagnosticSeverity.Info);
            UseCompareTag = CreateDiagnosticDescriptor<UseCompareTagResources>(DiagnosticIDs.UseCompareTag, DiagnosticCategories.GC, DiagnosticSeverity.Warning);
            UseNonAllocMethods = CreateDiagnosticDescriptor<UseNonAllocMethodsResources>(DiagnosticIDs.PhysicsUseNonAllocMethods, DiagnosticCategories.GC, DiagnosticSeverity.Warning, UnityVersion.UNITY_5_3);
            CameraMainIsSlow = CreateDiagnosticDescriptor<CameraMainResource>(DiagnosticIDs.CameraMainIsSlow, DiagnosticCategories.GC, DiagnosticSeverity.Warning);
            DoNotGCAllocnInUpdate = CreateDiagnosticDescriptor<DoNotGCAllocInUpdateResources>(DiagnosticIDs.DoNotGCAllocInUpdate, DiagnosticCategories.GC, DiagnosticSeverity.Warning);
            DoNotGCAllocnInUpdateRecursive = CreateDiagnosticDescriptor<DoNotGCAllocInUpdateResources>(DiagnosticIDs.DoNotGCAllocInUpdate, DiagnosticCategories.GC, DiagnosticSeverity.Warning);
            DoNotBoxWhenInvoke = CreateDiagnosticDescriptor<DoNotBoxWhenInvokeResource>(DiagnosticIDs.DoNotBoxWhenInvoke, DiagnosticCategories.GC, DiagnosticSeverity.Warning);
            ShouldCacheDelegate = CreateDiagnosticDescriptor<ShouldCacheDelegateResource>(DiagnosticIDs.ShouldCacheDelegate, DiagnosticCategories.GC, DiagnosticSeverity.Warning);

            //Performance
            DoNotUseFindMethodsInUpdate = CreateDiagnosticDescriptor<DoNotUseFindMethodsInUpdateResources>(DiagnosticIDs.DoNotUseFindMethodsInUpdate, DiagnosticCategories.Performance, DiagnosticSeverity.Warning);
            DoNotUseFindMethodsInUpdateRecursive = CreateDiagnosticDescriptor<DoNotUseFindMethodsInUpdateResources>(DiagnosticIDs.DoNotUseFindMethodsInUpdate, DiagnosticCategories.Performance, DiagnosticSeverity.Warning);
            DoNotUseForEachInUpdate = CreateDiagnosticDescriptor<DoNotUseForEachInUpdateResources>(DiagnosticIDs.DoNotUseForEachInUpdate, DiagnosticCategories.Performance, DiagnosticSeverity.Warning, UnityVersion.UNITY_1_0, UnityVersion.UNITY_5_5);
            UnsealedDerivedClass = CreateDiagnosticDescriptor<UnsealedDerivedClassResources>(DiagnosticIDs.UnsealedDerivedClass, DiagnosticCategories.Performance, DiagnosticSeverity.Warning);
            InvokeFunctionMissing = CreateDiagnosticDescriptor<InvokeFunctionMissingResources>(DiagnosticIDs.InvokeFunctionMissing, DiagnosticCategories.Performance, DiagnosticSeverity.Warning);
            DoNotUseStateName = CreateDiagnosticDescriptor<DoNotUseStateNameResource>(DiagnosticIDs.DoNotUseStateNameInAnimator, DiagnosticCategories.Performance, DiagnosticSeverity.Warning);
            DoNotUseStringPropertyNames = CreateDiagnosticDescriptor<DoNotUseStringPropertyNamesResource>(DiagnosticIDs.DoNotUseStringPropertyNamesInMaterial, DiagnosticCategories.Performance, DiagnosticSeverity.Warning);

            //Miscellaneous
            EmptyMonoBehaviourMethod = CreateDiagnosticDescriptor<EmptyMonoBehaviourMethodsResources>(DiagnosticIDs.EmptyMonoBehaviourMethod, DiagnosticCategories.Miscellaneous, DiagnosticSeverity.Warning);
            StructShouldImplementIEquatable = CreateDiagnosticDescriptor<StructAnalyzerResources>(DiagnosticIDs.StructShouldImplementIEquatable, DiagnosticCategories.Miscellaneous, DiagnosticSeverity.Warning);
            StructShouldOverrideEquals = CreateDiagnosticDescriptor<StructAnalyzerResources>(DiagnosticIDs.StructShouldOverrideEquals, DiagnosticCategories.Miscellaneous, DiagnosticSeverity.Warning);
            StructShouldOverrideGetHashCode = CreateDiagnosticDescriptor<StructAnalyzerResources>(DiagnosticIDs.StructShouldOverrideGetHashCode, DiagnosticCategories.Miscellaneous, DiagnosticSeverity.Warning);
            DoNotUseEnumTypeParameter = CreateDiagnosticDescriptor<DoNotUseEnumTypeParameterResource>(DiagnosticIDs.DoNoUseEnumTypeParameter, DiagnosticCategories.Miscellaneous, DiagnosticSeverity.Warning);

            //** AOT **
            DoNotUseRemoting = CreateDiagnosticDescriptor<DoNotUseRemotingResources>(DiagnosticIDs.DoNotUseRemoting, DiagnosticCategories.AOT, DiagnosticSeverity.Info);
            DoNotUseReflectionEmit = CreateDiagnosticDescriptor<DoNotUseReflectionEmitResources>(DiagnosticIDs.DoNotUseReflectionEmit, DiagnosticCategories.AOT, DiagnosticSeverity.Info);
            TypeGetType = CreateDiagnosticDescriptor<TypeGetTypeResources>(DiagnosticIDs.TypeGetType, DiagnosticCategories.AOT, DiagnosticSeverity.Info);
        }

        private static DiagnosticDescriptor CreateDiagnosticDescriptor<T>(string id, string category, DiagnosticSeverity severity, UnityVersion first = UnityVersion.UNITY_1_0, UnityVersion latest = UnityVersion.LATEST, bool isEnabledByDefault = true)
        {
            var resourceManager = new ResourceManager(typeof(T));

            return new DiagnosticDescriptor(
            id: id,
            title: new LocalizableResourceString("Title", resourceManager, typeof(T)),
            messageFormat: new LocalizableResourceString("MessageFormat", resourceManager, typeof(T)),
            category: category,
            defaultSeverity: severity,
            isEnabledByDefault: isEnabledByDefault,
            customTags: CreateUnityVersionInfo(first, latest),
            description: new LocalizableResourceString("Description", resourceManager, typeof(T)));
        }

        private static string[] CreateUnityVersionInfo(UnityVersion start, UnityVersion end)
        {
            return new string[] { Enum.GetName(typeof(UnityVersion), start), Enum.GetName(typeof(UnityVersion), end) };
        }

        public static UnityVersionSpan GetVersion(DiagnosticDescriptor dc)
        {
            var list = dc.CustomTags.ToList();

            if (list.Count < 2)
            {
                return new UnityVersionSpan(UnityVersion.NONE, UnityVersion.LATEST);
            }

            var start = (UnityVersion)Enum.Parse(typeof(UnityVersion), list[0]);
            var end = (UnityVersion)Enum.Parse(typeof(UnityVersion), list[1]);

            return new UnityVersionSpan(start, end);
        }
    }
}

