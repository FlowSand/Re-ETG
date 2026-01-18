// Decompiled with JetBrains decompiler
// Type: ResourceCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable

  public static class ResourceCache
  {
    private static Dictionary<string, Object> m_resourceCache = new Dictionary<string, Object>();

    public static Object Acquire(string resourceName)
    {
      if (!ResourceCache.m_resourceCache.ContainsKey(resourceName))
        ResourceCache.m_resourceCache.Add(resourceName, BraveResources.Load(resourceName));
      return ResourceCache.m_resourceCache[resourceName];
    }

    public static void ClearCache() => ResourceCache.m_resourceCache.Clear();
  }

