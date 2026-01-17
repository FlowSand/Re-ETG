// Decompiled with JetBrains decompiler
// Type: dfMarkupImageCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class dfMarkupImageCache
    {
      private static Dictionary<string, Texture> cache = new Dictionary<string, Texture>();

      public static void Clear() => dfMarkupImageCache.cache.Clear();

      public static void Load(string name, Texture image)
      {
        dfMarkupImageCache.cache[name.ToLowerInvariant()] = image;
      }

      public static void Unload(string name)
      {
        dfMarkupImageCache.cache.Remove(name.ToLowerInvariant());
      }

      public static Texture Load(string path)
      {
        path = path.ToLowerInvariant();
        if (dfMarkupImageCache.cache.ContainsKey(path))
          return dfMarkupImageCache.cache[path];
        Texture texture = BraveResources.Load(path) as Texture;
        if ((Object) texture != (Object) null)
          dfMarkupImageCache.cache[path] = texture;
        return texture;
      }
    }

}
