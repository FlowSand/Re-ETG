using System.Collections.Generic;
using UnityEngine;

#nullable disable

  public static class ShaderCache
  {
    private static Dictionary<string, Shader> m_shaderCache = new Dictionary<string, Shader>();

    public static Shader Acquire(string shaderName)
    {
      if (ShaderCache.m_shaderCache.ContainsKey(shaderName) && !(bool) (Object) ShaderCache.m_shaderCache[shaderName])
        ShaderCache.m_shaderCache.Remove(shaderName);
      if (!ShaderCache.m_shaderCache.ContainsKey(shaderName))
        ShaderCache.m_shaderCache.Add(shaderName, Shader.Find(shaderName));
      return ShaderCache.m_shaderCache[shaderName];
    }

    public static void ClearCache() => ShaderCache.m_shaderCache.Clear();
  }

