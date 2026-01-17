// Decompiled with JetBrains decompiler
// Type: Dungeonator.VisualStyleImpactData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Dungeonator;

[Serializable]
public class VisualStyleImpactData
{
  [SerializeField]
  public string annotation;
  [SerializeField]
  public GameObject[] wallShards;
  [SerializeField]
  public VFXComplex[] fallbackVerticalTileMapEffects;
  [SerializeField]
  public VFXComplex[] fallbackHorizontalTileMapEffects;

  public void SpawnRandomVertical(
    Vector3 position,
    float rotation,
    Transform enemy,
    Vector2 sourceNormal,
    Vector2 sourceVelocity)
  {
    VFXComplex verticalTileMapEffect = this.fallbackVerticalTileMapEffects[UnityEngine.Random.Range(0, this.fallbackVerticalTileMapEffects.Length)];
    float yPositionAtGround = (float) (Mathf.FloorToInt(position.y) - 1);
    verticalTileMapEffect.SpawnAtPosition(position.x, yPositionAtGround, position.y - yPositionAtGround, rotation, sourceNormal: new Vector2?(sourceNormal), sourceVelocity: new Vector2?(sourceVelocity));
  }

  public void SpawnRandomHorizontal(
    Vector3 position,
    float rotation,
    Transform enemy,
    Vector2 sourceNormal,
    Vector2 sourceVelocity)
  {
    this.fallbackHorizontalTileMapEffects[UnityEngine.Random.Range(0, this.fallbackHorizontalTileMapEffects.Length)].SpawnAtPosition(position, rotation, enemy, new Vector2?(sourceNormal), new Vector2?(sourceVelocity));
  }

  public void SpawnRandomShard(Vector3 position, Vector2 collisionNormal)
  {
    GameObject wallShard = this.wallShards[UnityEngine.Random.Range(0, this.wallShards.Length)];
    if (!((UnityEngine.Object) wallShard != (UnityEngine.Object) null))
      return;
    DebrisObject component = SpawnManager.SpawnDebris(wallShard, position, Quaternion.identity).GetComponent<DebrisObject>();
    component.angularVelocity = UnityEngine.Random.Range(0.5f, 1.5f) * component.angularVelocity;
    float num = (double) Mathf.Abs(collisionNormal.y) <= 0.10000000149011612 ? 0.0f : 0.25f;
    component.Trigger(Quaternion.Euler(0.0f, 0.0f, (float) UnityEngine.Random.Range(-30, 30)) * collisionNormal.ToVector3ZUp() * UnityEngine.Random.Range(0.0f, 4f), UnityEngine.Random.Range(0.1f, 0.5f) + num);
  }
}
