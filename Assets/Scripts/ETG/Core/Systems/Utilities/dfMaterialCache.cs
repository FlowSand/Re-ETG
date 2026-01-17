// Decompiled with JetBrains decompiler
// Type: dfMaterialCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    internal class dfMaterialCache
    {
      private static Dictionary<Material, dfMaterialCache.Cache> caches = new Dictionary<Material, dfMaterialCache.Cache>();

      public static void ForceUpdate(Material BaseMaterial)
      {
        dfMaterialCache.Cache cache = (dfMaterialCache.Cache) null;
        if (!dfMaterialCache.caches.TryGetValue(BaseMaterial, out cache))
          return;
        cache.Clear();
        cache.Reset();
      }

      public static Material Lookup(Material BaseMaterial)
      {
        if ((UnityEngine.Object) BaseMaterial == (UnityEngine.Object) null)
        {
          Debug.LogError((object) "Cache lookup on null material");
          return (Material) null;
        }
        dfMaterialCache.Cache cache1 = (dfMaterialCache.Cache) null;
        if (!dfMaterialCache.caches.TryGetValue(BaseMaterial, out cache1))
        {
          dfMaterialCache.Cache cache2 = new dfMaterialCache.Cache(BaseMaterial);
          dfMaterialCache.caches[BaseMaterial] = cache2;
          cache1 = cache2;
        }
        return cache1.Obtain();
      }

      public static void Reset() => dfMaterialCache.Cache.ResetAll();

      public static void Clear()
      {
        dfMaterialCache.Cache.ClearAll();
        dfMaterialCache.caches.Clear();
      }

      private class Cache
      {
        private static List<dfMaterialCache.Cache> cacheInstances = new List<dfMaterialCache.Cache>();
        private Material baseMaterial;
        private List<Material> instances = new List<Material>(10);
        private int currentIndex;

        private Cache() => throw new NotImplementedException();

        public Cache(Material BaseMaterial)
        {
          this.baseMaterial = BaseMaterial;
          this.instances.Add(BaseMaterial);
          dfMaterialCache.Cache.cacheInstances.Add(this);
        }

        public static void ClearAll()
        {
          for (int index = 0; index < dfMaterialCache.Cache.cacheInstances.Count; ++index)
            dfMaterialCache.Cache.cacheInstances[index].Clear();
          dfMaterialCache.Cache.cacheInstances.Clear();
        }

        public static void ResetAll()
        {
          for (int index = 0; index < dfMaterialCache.Cache.cacheInstances.Count; ++index)
            dfMaterialCache.Cache.cacheInstances[index].Reset();
        }

        public Material Obtain()
        {
          if (this.currentIndex < this.instances.Count)
            return this.instances[this.currentIndex++];
          ++this.currentIndex;
          Material material1 = new Material(this.baseMaterial);
          material1.hideFlags = HideFlags.DontSave | HideFlags.HideInInspector;
          material1.name = $"{this.baseMaterial.name} (Copy {this.currentIndex})";
          Material material2 = material1;
          this.instances.Add(material2);
          return material2;
        }

        public void Reset() => this.currentIndex = 0;

        public void Clear()
        {
          this.currentIndex = 0;
          for (int index = 1; index < this.instances.Count; ++index)
          {
            Material instance = this.instances[index];
            if ((UnityEngine.Object) instance != (UnityEngine.Object) null)
            {
              if (Application.isPlaying)
                UnityEngine.Object.Destroy((UnityEngine.Object) instance);
              else
                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) instance);
            }
          }
          this.instances.Clear();
        }
      }
    }

}
