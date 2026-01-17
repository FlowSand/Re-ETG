// Decompiled with JetBrains decompiler
// Type: HighPriestChallengeModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class HighPriestChallengeModifier : ChallengeModifier
{
  [EnemyIdentifier]
  public string CandleGuid;
  public int NumCandles = 6;
  public float MergoCooldown = 25f;
  private AIActor m_boss;
  private RoomHandler m_room;

  private void Start()
  {
    this.m_room = GameManager.Instance.PrimaryPlayer.CurrentRoom;
    List<AIActor> activeEnemies = this.m_room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
    for (int index = 0; index < activeEnemies.Count; ++index)
    {
      if ((bool) (UnityEngine.Object) activeEnemies[index] && (bool) (UnityEngine.Object) activeEnemies[index].healthHaver && activeEnemies[index].healthHaver.IsBoss)
        this.m_boss = activeEnemies[index];
    }
    if ((bool) (UnityEngine.Object) this.m_boss.behaviorSpeculator)
    {
      for (int index1 = 0; index1 < this.m_boss.behaviorSpeculator.AttackBehaviors.Count; ++index1)
      {
        if (this.m_boss.behaviorSpeculator.AttackBehaviors[index1] is AttackBehaviorGroup)
        {
          AttackBehaviorGroup attackBehavior = this.m_boss.behaviorSpeculator.AttackBehaviors[index1] as AttackBehaviorGroup;
          for (int index2 = 0; index2 < attackBehavior.AttackBehaviors.Count; ++index2)
          {
            if (attackBehavior.AttackBehaviors[index2].Behavior is HighPriestMergoBehavior)
            {
              attackBehavior.AttackBehaviors[index2].Probability = 1000f;
              (attackBehavior.AttackBehaviors[index2].Behavior as HighPriestMergoBehavior).Cooldown = this.MergoCooldown;
            }
          }
        }
      }
    }
    this.m_room.OnChangedTerrifyingDarkState += new Action<bool>(this.HandleDarkStateChange);
  }

  private void HandleDarkStateChange(bool isDark)
  {
    if (isDark)
      return;
    this.SpawnWave();
  }

  private void OnDestroy()
  {
    DeadlyDeadlyGoopManager.DelayedClearGoopsInRadius(GameManager.Instance.PrimaryPlayer.CenterPosition, 100f);
  }

  private void SpawnWave()
  {
    int numCandles = this.NumCandles;
    for (int index = 0; index < numCandles; ++index)
    {
      AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(this.CandleGuid);
      IntVector2? nullable = this.PreprocessSpawn(orLoadByGuid, this.m_boss.specRigidbody.UnitCenter, this.m_room);
      if (nullable.HasValue)
        AIActor.Spawn(orLoadByGuid, nullable.Value, this.m_room, true);
    }
  }

  private IntVector2? PreprocessSpawn(AIActor enemy, Vector2 center, RoomHandler sourceRoom)
  {
    PixelCollider groundPixelCollider = enemy.specRigidbody.GroundPixelCollider;
    IntVector2 m_enemyClearance;
    if (groundPixelCollider != null && groundPixelCollider.ColliderGenerationMode == PixelCollider.PixelColliderGeneration.Manual)
    {
      m_enemyClearance = new Vector2((float) groundPixelCollider.ManualWidth / 16f, (float) groundPixelCollider.ManualHeight / 16f).ToIntVector2(VectorConversions.Ceil);
    }
    else
    {
      Debug.LogFormat("Enemy type {0} does not have a manually defined ground collider!", (object) enemy.name);
      m_enemyClearance = IntVector2.One;
    }
    float minDistanceSquared = 0.0f;
    float maxDistanceSquared = 400f;
    CellValidator cellValidator = (CellValidator) (c =>
    {
      for (int index1 = 0; index1 < m_enemyClearance.x; ++index1)
      {
        for (int index2 = 0; index2 < m_enemyClearance.y; ++index2)
        {
          if (GameManager.Instance.Dungeon.data.isTopWall(c.x + index1, c.y + index2))
            return false;
        }
      }
      float num1 = (float) c.x + 0.5f - center.x;
      float num2 = (float) c.y + 0.5f - center.y;
      float num3 = (float) ((double) num1 * (double) num1 + (double) num2 * (double) num2);
      return (double) num3 >= (double) minDistanceSquared && (double) num3 <= (double) maxDistanceSquared;
    });
    return sourceRoom.GetRandomAvailableCell(new IntVector2?(m_enemyClearance), new CellTypes?(enemy.PathableTiles), true, cellValidator);
  }
}
