// Decompiled with JetBrains decompiler
// Type: PathologicalGames.PreRuntimePoolItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace PathologicalGames;

[AddComponentMenu("Path-o-logical/PoolManager/Pre-Runtime Pool Item")]
public class PreRuntimePoolItem : MonoBehaviour
{
  public string poolName = string.Empty;
  public string prefabName = string.Empty;
  public bool despawnOnStart = true;
  public bool doNotReparent;

  private void Start()
  {
    SpawnPool spawnPool;
    if (!PoolManager.Pools.TryGetValue(this.poolName, out spawnPool))
      Debug.LogError((object) $"PreRuntimePoolItem Error ('{this.name}'): No pool with the name '{this.poolName}' exists! Create one using the PoolManager Inspector interface or PoolManager.CreatePool().See the online docs for more information at http://docs.poolmanager.path-o-logical.com");
    else
      spawnPool.Add(this.transform, this.prefabName, this.despawnOnStart, !this.doNotReparent);
  }
}
