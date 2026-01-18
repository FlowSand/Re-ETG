// Decompiled with JetBrains decompiler
// Type: dfAtlas
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/User Interface/Texture Atlas")]
[ExecuteInEditMode]
[Serializable]
public class dfAtlas : MonoBehaviour
  {
    [SerializeField]
    protected Material material;
    [SerializeField]
    protected List<dfAtlas.ItemInfo> items = new List<dfAtlas.ItemInfo>();
    public dfAtlas.TextureAtlasGenerator generator;
    public string imageFileGUID;
    public string dataFileGUID;
    private Dictionary<string, dfAtlas.ItemInfo> map = new Dictionary<string, dfAtlas.ItemInfo>();
    private dfAtlas replacementAtlas;

    public Texture2D Texture
    {
      get
      {
        return (UnityEngine.Object) this.replacementAtlas != (UnityEngine.Object) null ? this.replacementAtlas.Texture : this.material.mainTexture as Texture2D;
      }
    }

    public int Count
    {
      get
      {
        return (UnityEngine.Object) this.replacementAtlas != (UnityEngine.Object) null ? this.replacementAtlas.Count : this.items.Count;
      }
    }

    public List<dfAtlas.ItemInfo> Items
    {
      get
      {
        return (UnityEngine.Object) this.replacementAtlas != (UnityEngine.Object) null ? this.replacementAtlas.Items : this.items;
      }
    }

    public Material Material
    {
      get
      {
        return (UnityEngine.Object) this.replacementAtlas != (UnityEngine.Object) null ? this.replacementAtlas.Material : this.material;
      }
      set
      {
        if ((UnityEngine.Object) this.replacementAtlas != (UnityEngine.Object) null)
          this.replacementAtlas.Material = value;
        else
          this.material = value;
      }
    }

    public dfAtlas Replacement
    {
      get => this.replacementAtlas;
      set => this.replacementAtlas = value;
    }

    public dfAtlas.ItemInfo this[string key]
    {
      get
      {
        if ((UnityEngine.Object) this.replacementAtlas != (UnityEngine.Object) null)
          return this.replacementAtlas[key];
        if (string.IsNullOrEmpty(key))
          return (dfAtlas.ItemInfo) null;
        if (this.map.Count == 0)
          this.RebuildIndexes();
        dfAtlas.ItemInfo itemInfo = (dfAtlas.ItemInfo) null;
        return this.map.TryGetValue(key, out itemInfo) ? itemInfo : (dfAtlas.ItemInfo) null;
      }
    }

    internal static bool Equals(dfAtlas lhs, dfAtlas rhs)
    {
      if (object.ReferenceEquals((object) lhs, (object) rhs))
        return true;
      return !((UnityEngine.Object) lhs == (UnityEngine.Object) null) && !((UnityEngine.Object) rhs == (UnityEngine.Object) null) && (UnityEngine.Object) lhs.material == (UnityEngine.Object) rhs.material;
    }

    public void AddItem(dfAtlas.ItemInfo item)
    {
      this.items.Add(item);
      this.RebuildIndexes();
    }

    public void AddItems(IEnumerable<dfAtlas.ItemInfo> list)
    {
      this.items.AddRange(list);
      this.RebuildIndexes();
    }

    public void Remove(string name)
    {
      for (int index = this.items.Count - 1; index >= 0; --index)
      {
        if (this.items[index].name == name)
          this.items.RemoveAt(index);
      }
      this.RebuildIndexes();
    }

    public void RebuildIndexes()
    {
      if (this.map == null)
        this.map = new Dictionary<string, dfAtlas.ItemInfo>();
      else
        this.map.Clear();
      for (int index = 0; index < this.items.Count; ++index)
      {
        dfAtlas.ItemInfo itemInfo = this.items[index];
        this.map[itemInfo.name] = itemInfo;
      }
    }

    public enum TextureAtlasGenerator
    {
      Internal,
      TexturePacker,
    }

    [Serializable]
    public class ItemInfo : IComparable<dfAtlas.ItemInfo>, IEquatable<dfAtlas.ItemInfo>
    {
      public string name;
      public Rect region;
      public RectOffset border = new RectOffset();
      public bool rotated;
      public Vector2 sizeInPixels = Vector2.zero;
      [SerializeField]
      public string textureGUID = string.Empty;
      public bool deleted;
      [SerializeField]
      public Texture2D texture;

      public int CompareTo(dfAtlas.ItemInfo other) => this.name.CompareTo(other.name);

      public override int GetHashCode() => this.name.GetHashCode();

      public override bool Equals(object obj)
      {
        return (object) (obj as dfAtlas.ItemInfo) != null && this.name.Equals(((dfAtlas.ItemInfo) obj).name);
      }

      public bool Equals(dfAtlas.ItemInfo other) => this.name.Equals(other.name);

      public static bool operator ==(dfAtlas.ItemInfo lhs, dfAtlas.ItemInfo rhs)
      {
        if (object.ReferenceEquals((object) lhs, (object) rhs))
          return true;
        return (object) lhs != null && (object) rhs != null && lhs.name.Equals(rhs.name);
      }

      public static bool operator !=(dfAtlas.ItemInfo lhs, dfAtlas.ItemInfo rhs) => !(lhs == rhs);
    }
  }

