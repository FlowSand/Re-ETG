// Decompiled with JetBrains decompiler
// Type: tk2dSystem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class tk2dSystem : ScriptableObject
{
  public const string guidPrefix = "tk2d/tk2d_";
  public const string assetName = "tk2d/tk2dSystem";
  public const string assetFileName = "tk2dSystem.asset";
  [NonSerialized]
  public tk2dAssetPlatform[] assetPlatforms = new tk2dAssetPlatform[3]
  {
    new tk2dAssetPlatform("1x", 1f),
    new tk2dAssetPlatform("2x", 2f),
    new tk2dAssetPlatform("4x", 4f)
  };
  private static tk2dSystem _inst;
  private static string currentPlatform = string.Empty;
  [SerializeField]
  private tk2dResourceTocEntry[] allResourceEntries = new tk2dResourceTocEntry[0];

  private tk2dSystem()
  {
  }

  public static tk2dSystem inst
  {
    get
    {
      if ((UnityEngine.Object) tk2dSystem._inst == (UnityEngine.Object) null)
      {
        tk2dSystem._inst = BraveResources.Load("tk2d/tk2dSystem", typeof (tk2dSystem)) as tk2dSystem;
        if ((UnityEngine.Object) tk2dSystem._inst == (UnityEngine.Object) null)
          tk2dSystem._inst = ScriptableObject.CreateInstance<tk2dSystem>();
        UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object) tk2dSystem._inst);
      }
      return tk2dSystem._inst;
    }
  }

  public static tk2dSystem inst_NoCreate
  {
    get
    {
      if ((UnityEngine.Object) tk2dSystem._inst == (UnityEngine.Object) null)
        tk2dSystem._inst = BraveResources.Load("tk2d/tk2dSystem", typeof (tk2dSystem)) as tk2dSystem;
      return tk2dSystem._inst;
    }
  }

  public static string CurrentPlatform
  {
    get => tk2dSystem.currentPlatform;
    set
    {
      if (!(value != tk2dSystem.currentPlatform))
        return;
      tk2dSystem.currentPlatform = value;
    }
  }

  public static bool OverrideBuildMaterial => false;

  public static tk2dAssetPlatform GetAssetPlatform(string platform)
  {
    tk2dSystem instNoCreate = tk2dSystem.inst_NoCreate;
    if ((UnityEngine.Object) instNoCreate == (UnityEngine.Object) null)
      return (tk2dAssetPlatform) null;
    for (int index = 0; index < instNoCreate.assetPlatforms.Length; ++index)
    {
      if (instNoCreate.assetPlatforms[index].name == platform)
        return instNoCreate.assetPlatforms[index];
    }
    return (tk2dAssetPlatform) null;
  }

  private T LoadResourceByGUIDImpl<T>(string guid) where T : UnityEngine.Object
  {
    tk2dResource tk2dResource = BraveResources.Load("tk2d/tk2d_" + guid, typeof (tk2dResource)) as tk2dResource;
    return (UnityEngine.Object) tk2dResource != (UnityEngine.Object) null ? tk2dResource.objectReference as T : (T) null;
  }

  private T LoadResourceByNameImpl<T>(string name) where T : UnityEngine.Object
  {
    for (int index = 0; index < this.allResourceEntries.Length; ++index)
    {
      if (this.allResourceEntries[index] != null && this.allResourceEntries[index].assetName == name)
        return this.LoadResourceByGUIDImpl<T>(this.allResourceEntries[index].assetGUID);
    }
    return (T) null;
  }

  public static T LoadResourceByGUID<T>(string guid) where T : UnityEngine.Object
  {
    return tk2dSystem.inst.LoadResourceByGUIDImpl<T>(guid);
  }

  public static T LoadResourceByName<T>(string guid) where T : UnityEngine.Object
  {
    return tk2dSystem.inst.LoadResourceByNameImpl<T>(guid);
  }
}
