// Decompiled with JetBrains decompiler
// Type: BraveResources
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BraveResources
{
  private static AssetBundle m_assetBundle;

  public static UnityEngine.Object Load(string path, string extension = ".prefab")
  {
    if ((UnityEngine.Object) BraveResources.m_assetBundle == (UnityEngine.Object) null)
      BraveResources.EnsureLoaded();
    return BraveResources.m_assetBundle.LoadAsset<UnityEngine.Object>($"assets/ResourcesBundle/{path}{extension}");
  }

  public static UnityEngine.Object Load(string path, System.Type type, string extension = ".prefab")
  {
    if ((UnityEngine.Object) BraveResources.m_assetBundle == (UnityEngine.Object) null)
      BraveResources.EnsureLoaded();
    return BraveResources.m_assetBundle.LoadAsset($"assets/ResourcesBundle/{path}{extension}", type);
  }

  public static T Load<T>(string path, string extension = ".prefab") where T : UnityEngine.Object
  {
    if ((UnityEngine.Object) BraveResources.m_assetBundle == (UnityEngine.Object) null)
      BraveResources.EnsureLoaded();
    return BraveResources.m_assetBundle.LoadAsset<T>($"assets/ResourcesBundle/{path}{extension}");
  }

  public static void EnsureLoaded()
  {
    if (!((UnityEngine.Object) BraveResources.m_assetBundle == (UnityEngine.Object) null))
      return;
    BraveResources.m_assetBundle = ResourceManager.LoadAssetBundle("brave_resources_001");
  }
}
