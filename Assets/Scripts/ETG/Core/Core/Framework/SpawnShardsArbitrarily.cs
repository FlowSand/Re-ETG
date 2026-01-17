// Decompiled with JetBrains decompiler
// Type: SpawnShardsArbitrarily
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class SpawnShardsArbitrarily : BraveBehaviour
    {
      public MinorBreakable.BreakStyle breakStyle;
      public ShardCluster[] shardClusters;
      public float heightOffGround = 0.1f;
      public bool TriggerOnDestroy;
      public bool TriggerOnDamaged;

      private void Start()
      {
        if (!this.TriggerOnDamaged)
          return;
        this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.HandleDamaged);
      }

      private void HandleDamaged(
        float resultValue,
        float maxValue,
        CoreDamageTypes damageTypes,
        DamageCategory damageCategory,
        Vector2 damageDirection)
      {
        this.HandleShardSpawns(damageDirection.normalized);
      }

      protected override void OnDestroy()
      {
        if (this.TriggerOnDestroy)
          this.HandleShardSpawns(Vector2.zero);
        base.OnDestroy();
      }

      private void HandleShardSpawns(Vector2 sourceVelocity)
      {
        MinorBreakable.BreakStyle breakStyle = this.breakStyle;
        if (sourceVelocity == Vector2.zero)
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
        }
      }

      public void SpawnShards(
        Vector2 direction,
        float minAngle,
        float maxAngle,
        float verticalSpeed,
        float minMagnitude,
        float maxMagnitude)
      {
        Vector3 position = !(bool) (Object) this.specRigidbody ? (!(bool) (Object) this.sprite ? this.transform.position : this.sprite.WorldCenter.ToVector3ZisY()) : this.specRigidbody.GetUnitCenter(ColliderType.HitBox).ToVector3ZisY();
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
            if ((bool) (Object) gameObject)
            {
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
    }

}
