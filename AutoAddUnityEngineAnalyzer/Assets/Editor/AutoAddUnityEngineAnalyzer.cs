
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace UnityEngineAnalyzer
{

    public class AutoAddUnityEngineAnalyzer : AssetPostprocessor
    {
        /// <summary>
        /// Put your UnityEngineAnalyzer.dll in your unity project,
        /// and modify this path relative to your project.
        /// </summary>
        public const string UnityEngineAnalyzerPath = "Tools\\VisualStudio\\UnityEngineAnalyzer\\UnityEngineAnalyzer.dll";
        
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            TryAddUnityEngineAnalyzer();
        }
       

        private static void TryAddUnityEngineAnalyzer()
        {
            string dataPath = Application.dataPath + "/../";

            var csprojPaths = Directory.GetFiles(dataPath, "*.csproj");

            foreach(var oneCsProjPath in csprojPaths)
            {
                if(!string.IsNullOrEmpty(oneCsProjPath))
                {
                    XDocument doc = XDocument.Load(oneCsProjPath);
                    var defaultNamespace = doc.Root.GetDefaultNamespace();

                    var unityEngineAnalyzer = doc.Descendants(defaultNamespace + "Analyzer").
                        Where(x => x.Attribute("Include").
                            Value.Contains("UnityEngineAnalyzer.dll")).
                        FirstOrDefault();

                    if(unityEngineAnalyzer == null)
                    {
                        Debug.Log("can not find UnityEngineAnalyzer in oneCsProjPath=" + oneCsProjPath);

                        try
                        {
                            doc.Root.
                            Add(
                                new XElement(defaultNamespace + "ItemGroup",
                                    new XElement(defaultNamespace + "Analyzer",
                                        new XAttribute("Include", UnityEngineAnalyzerPath))));

                            doc.Save(oneCsProjPath);
                            Debug.Log("did add UnityEngineAnalyzer in oneCsProjPath=" + oneCsProjPath);
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError("exception caught in adding UnityEngineAnalyzer in oneCsProjPath=" + oneCsProjPath + "\nexception=" + ex);
                        }
                    }
                }
            }
        }
    }

}