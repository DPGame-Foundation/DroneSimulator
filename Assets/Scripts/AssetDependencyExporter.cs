using UnityEditor;
using UnityEngine;
using System.IO;

public class AssetDependencyExporter
{
    [MenuItem("Tools/Export Asset Dependencies")]
    public static void ExportDependencies()
    {
        string outputPath = "Assets/AssetDependencies.txt";
        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            foreach (string path in assetPaths)
            {
                // Optionally filter for specific asset types, e.g., prefabs, scripts
                if (path.EndsWith(".prefab") || path.EndsWith(".cs"))
                {
                    writer.WriteLine(path);
                }
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("Asset dependencies exported to " + outputPath);
    }
}
