// Decompiled with JetBrains decompiler
// Type: ShardCluster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public class ShardCluster
{
  public int minFromCluster = 1;
  public int maxFromCluster = 3;
  public float forceMultiplier = 1f;
  public Vector3 forceAxialMultiplier = Vector3.one;
  public float rotationMultiplier = 1f;
  public DebrisObject[] clusterObjects;

  public void SpawnShards(
    Vector2 position,
    Vector2 direction,
    float minAngle,
    float maxAngle,
    float verticalSpeed,
    float minMagnitude,
    float maxMagnitude,
    tk2dSprite sourceSprite)
  {
    int num1 = UnityEngine.Random.Range(this.minFromCluster, this.maxFromCluster + 1);
    int num2 = UnityEngine.Random.Range(0, this.clusterObjects.Length);
    int iterator = 0;
    for (int index = 0; index < num1; ++index)
    {
      float discrepancyRandom = BraveMathCollege.GetLowDiscrepancyRandom(iterator);
      ++iterator;
      Vector3 a = Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(minAngle, maxAngle, discrepancyRandom)) * (direction.normalized * UnityEngine.Random.Range(minMagnitude, maxMagnitude)).ToVector3ZUp(verticalSpeed);
      GameObject gameObject = SpawnManager.SpawnDebris(this.clusterObjects[(num2 + index) % this.clusterObjects.Length].gameObject, (Vector3) position, Quaternion.identity);
      tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
      if ((UnityEngine.Object) sourceSprite != (UnityEngine.Object) null && (UnityEngine.Object) sourceSprite.attachParent != (UnityEngine.Object) null && (UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        component.attachParent = sourceSprite.attachParent;
        component.HeightOffGround = sourceSprite.HeightOffGround;
      }
      gameObject.GetComponent<DebrisObject>().Trigger(Vector3.Scale(a, this.forceAxialMultiplier) * this.forceMultiplier, 0.5f, this.rotationMultiplier);
    }
  }
}
