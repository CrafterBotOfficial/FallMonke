using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace FallMonke;

public class AssetLoader
{
    public AssetBundle bundle;

    public AssetLoader(string resourcePath)
    {
        using Stream assetReaderStream = typeof(AssetLoader).Assembly.GetManifestResourceStream(resourcePath);
        bundle = AssetBundle.LoadFromStream(assetReaderStream);
    }

    public string GetSceneName()
    {
        var paths = bundle.GetAllScenePaths();
        if (paths.Length == 0 || paths[0].IsNullOrEmpty())
        {
            Main.Log("Failed to find scene in assetbundle", BepInEx.Logging.LogLevel.Fatal);
            return string.Empty;
        }

        return paths[0];
    }
}
