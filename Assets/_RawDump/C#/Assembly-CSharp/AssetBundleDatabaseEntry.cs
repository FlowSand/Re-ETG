// Decompiled with JetBrains decompiler
// Type: AssetBundleDatabaseEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public abstract class AssetBundleDatabaseEntry
{
  public string myGuid;
  public string unityGuid;
  public string path;
  [NonSerialized]
  protected UnityEngine.Object loadedPrefab;

  public abstract AssetBundle assetBundle { get; }

  public string name
  {
    get => this.path.Substring(this.path.LastIndexOf('/') + 1).Replace(".prefab", string.Empty);
  }

  public bool HasLoadedPrefab => (bool) this.loadedPrefab;

  public virtual void DropReference() => this.loadedPrefab = (UnityEngine.Object) null;
}
