// Decompiled with JetBrains decompiler
// Type: SpawnShardsOnDeath
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class SpawnShardsOnDeath : OnDeathBehavior
  {
    public MinorBreakable.BreakStyle breakStyle;
    [ShowInInspectorIf("breakStyle", 4, true)]
    public Vector2 direction;
    [ShowInInspectorIf("breakStyle", 4, true)]
    public float minAngle;
    [ShowInInspectorIf("breakStyle", 4, true)]
    public float maxAngle;
    [ShowInInspectorIf("breakStyle", 4, true)]
    public float verticalSpeed;
    [ShowInInspectorIf("breakStyle", 4, true)]
    public float minMagnitude;
    [ShowInInspectorIf("breakStyle", 4, true)]
    public float maxMagnitude;
    public ShardCluster[] shardClusters;
    public float heightOffGround = 0.1f;

    protected override void OnDestroy() => base.OnDestroy();

    protected override void OnTrigger(Vector2 deathVelocity) => this.HandleShardSpawns(deathVelocity);

    public void HandleShardSpawns(Vector2 sourceVelocity, Vector2? spawnPos = null)
    {
      MinorBreakable.BreakStyle breakStyle = this.breakStyle;
      if (sourceVelocity == Vector2.zero && this.breakStyle != MinorBreakable.BreakStyle.CUSTOM)
        breakStyle = MinorBreakable.BreakStyle.BURST;
      float verticalSpeed = 1.5f;
      switch (breakStyle)
      {
        case MinorBreakable.BreakStyle.CONE:
          this.SpawnShards(sourceVelocity, -45f, 45f, verticalSpeed, sourceVelocity.magnitude * 0.5f, sourceVelocity.magnitude * 1.5f);
          break;
        case MinorBreakable.BreakStyle.BURST:
          this.SpawnShards(Vector2.right, -180f, 180f, verticalSpeed, 1f, 2f);
          break;
        case MinorBreakable.BreakStyle.JET:
          this.SpawnShards(sourceVelocity, -15f, 15f, verticalSpeed, sourceVelocity.magnitude * 0.5f, sourceVelocity.magnitude * 1.5f);
          break;
        case MinorBreakable.BreakStyle.CUSTOM:
          this.SpawnShards(this.direction, this.minAngle, this.maxAngle, this.verticalSpeed, this.minMagnitude, this.maxMagnitude, spawnPos);
          break;
      }
    }

    public void SpawnShards(
      Vector2 direction,
      float minAngle,
      float maxAngle,
      float verticalSpeed,
      float minMagnitude,
      float maxMagnitude,
      Vector2? spawnPos = null)
    {
      Vector3 position = (Vector3) (!spawnPos.HasValue ? this.specRigidbody.GetUnitCenter(ColliderType.HitBox) : spawnPos.Value);
      if (this.shardClusters == null || this.shardClusters.Length <= 0)
        return;
      int iterator = Random.Range(0, 10);
      for (int index1 = 0; index1 < this.shardClusters.Length; ++index1)
      {
        ShardCluster shardCluster = this.shardClusters[index1];
        int num1 = Random.Range(shardCluster.minFromCluster, shardCluster.maxFromCluster + 1);
        int num2 = Random.Range(0, shardCluster.clusterObjects.Length);
        for (int index2 = 0; index2 < num1; ++index2)
        {
          float discrepancyRandom = BraveMathCollege.GetLowDiscrepancyRandom(iterator);
          ++iterator;
          Vector3 a = Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(minAngle, maxAngle, discrepancyRandom)) * (direction.normalized * Random.Range(minMagnitude, maxMagnitude)).ToVector3ZUp(verticalSpeed);
          int index3 = (num2 + index2) % shardCluster.clusterObjects.Length;
          GameObject gameObject = SpawnManager.SpawnDebris(shardCluster.clusterObjects[index3].gameObject, position, Quaternion.identity);
          tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
          if ((Object) this.sprite.attachParent != (Object) null && (Object) component != (Object) null)
          {
            component.attachParent = this.sprite.attachParent;
            component.HeightOffGround = this.sprite.HeightOffGround;
          }
          gameObject.GetComponent<DebrisObject>().Trigger(Vector3.Scale(a, shardCluster.forceAxialMultiplier) * shardCluster.forceMultiplier, this.heightOffGround, shardCluster.rotationMultiplier);
        }
      }
    }
  }

