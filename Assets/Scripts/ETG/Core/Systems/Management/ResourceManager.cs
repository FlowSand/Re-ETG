using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable

public class ResourceManager
  {
    private static Dictionary<string, AssetBundle> LoadedBundles;
    private static string[] BundlePrereqs = new string[6]
    {
      "shared_base_001",
      "shared_auto_001",
      "shared_auto_002",
      "brave_resources_001",
      "enemies_base_001",
      "dungeons/base_foyer"
    };

    public static void Init()
    {
      if (ResourceManager.LoadedBundles != null)
        return;
      ResourceManager.LoadedBundles = new Dictionary<string, AssetBundle>();
      for (int index = 0; index < ResourceManager.BundlePrereqs.Length; ++index)
        ResourceManager.LoadAssetBundle(ResourceManager.BundlePrereqs[index]);
    }

    [DebuggerHidden]
    public static IEnumerator InitAsync()
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      ResourceManager__InitAsyncc__Iterator0 initAsyncCIterator0 = new ResourceManager__InitAsyncc__Iterator0();
      return (IEnumerator) initAsyncCIterator0;
    }

    public static AssetBundle LoadAssetBundle(string path)
    {
      if (ResourceManager.LoadedBundles == null)
        ResourceManager.Init();
      AssetBundle assetBundle;
      if (ResourceManager.LoadedBundles.TryGetValue(path, out assetBundle))
        return assetBundle;
      string path1 = Path.Combine(Application.streamingAssetsPath, Path.Combine("Assets/", path));
      DebugTime.RecordStartTime();
      assetBundle = AssetBundle.LoadFromFile(path1);
      DebugTime.Log("AssetBundle.LoadFromFile({0})", (object) path);
      ResourceManager.LoadedBundles.Add(path, assetBundle);
      return assetBundle;
    }

    public static void LoadSceneFromBundle(AssetBundle assetBundle, LoadSceneMode mode)
    {
      SceneManager.LoadScene(ResourceManager.GetSceneName(assetBundle), mode);
    }

    public static AsyncOperation LoadSceneAsyncFromBundle(AssetBundle assetBundle, LoadSceneMode mode)
    {
      return SceneManager.LoadSceneAsync(ResourceManager.GetSceneName(assetBundle), mode);
    }

    public static void LoadLevelFromBundle(AssetBundle assetBundle)
    {
      Application.LoadLevel(ResourceManager.GetSceneName(assetBundle));
    }

    private static string GetSceneName(AssetBundle assetBundle) => assetBundle.GetAllScenePaths()[0];
  }

