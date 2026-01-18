// Decompiled with JetBrains decompiler
// Type: DatabaseEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

[Serializable]
public class DatabaseEntry
  {
    public string myGuid;
    public string unityGuid;
    public string path;
    [NonSerialized]
    private UnityEngine.Object loadedPrefab;

    public string name
    {
      get => this.path.Substring(this.path.LastIndexOf('/') + 1).Replace(".prefab", string.Empty);
    }

    public bool HasLoadedPrefab => (bool) this.loadedPrefab;

    public T GetPrefab<T>() where T : UnityEngine.Object
    {
      if (!(bool) this.loadedPrefab)
      {
        if (!this.path.StartsWith("Assets/Resources/"))
        {
          Debug.LogErrorFormat("Trying to instantate an object that doesn't live in Resources! {0} {1} {2}", (object) this.myGuid, (object) this.unityGuid, (object) this.path);
          return (T) null;
        }
        this.loadedPrefab = (UnityEngine.Object) BraveResources.Load<T>(this.path.Replace("Assets/Resources/", string.Empty).Replace(".prefab", string.Empty));
      }
      return this.loadedPrefab as T;
    }

    public void DropReference() => this.loadedPrefab = (UnityEngine.Object) null;
  }

