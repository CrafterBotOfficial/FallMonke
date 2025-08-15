﻿using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace FallMonke;

public class AssetLoader : IDisposable
{
    private AssetBundle bundle;

    public AssetLoader(string resourcePath)
    {
        using Stream assetReaderStream = typeof(AssetLoader).Assembly.GetManifestResourceStream(resourcePath);
        bundle = AssetBundle.LoadFromStream(assetReaderStream);
    }

    public async Task<UnityEngine.Object> LoadAsset(string name)
    {
        var request = bundle.LoadAssetAsync(name);
        await Task.Run(() => request.completed += _ => { });
        return request.asset;
    }

    public void Dispose()
    {
        bundle.Unload(false);
    }
}
