using System.Collections.Generic;
using UnityEngine;

#nullable disable

  public static class GlobalDispersalParticleManager
  {
    public static Dictionary<GameObject, ParticleSystem> PrefabToSystemMap;

    public static ParticleSystem GetSystemForPrefab(GameObject prefab)
    {
      if (GlobalDispersalParticleManager.PrefabToSystemMap == null)
        GlobalDispersalParticleManager.PrefabToSystemMap = new Dictionary<GameObject, ParticleSystem>();
      if (GlobalDispersalParticleManager.PrefabToSystemMap.ContainsKey(prefab))
        return GlobalDispersalParticleManager.PrefabToSystemMap[prefab];
      ParticleSystem component = Object.Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
      GlobalDispersalParticleManager.PrefabToSystemMap.Add(prefab, component);
      return component;
    }

    public static void Clear()
    {
      if (GlobalDispersalParticleManager.PrefabToSystemMap == null)
        return;
      GlobalDispersalParticleManager.PrefabToSystemMap.Clear();
    }
  }

