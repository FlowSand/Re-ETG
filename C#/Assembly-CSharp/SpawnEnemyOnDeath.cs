// Decompiled with JetBrains decompiler
// Type: SpawnEnemyOnDeath
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

#nullable disable
public class SpawnEnemyOnDeath : OnDeathBehavior
{
  public float chanceToSpawn = 1f;
  public string spawnVfx;
  [Header("Enemies to Spawn")]
  public SpawnEnemyOnDeath.EnemySelection enemySelection = SpawnEnemyOnDeath.EnemySelection.All;
  [EnemyIdentifier]
  public string[] enemyGuidsToSpawn;
  [ShowInInspectorIf("ShowRandomPrams", true)]
  public int minSpawnCount = 1;
  [ShowInInspectorIf("ShowRandomPrams", true)]
  public int maxSpawnCount = 1;
  [FormerlySerializedAs("spawnType")]
  [Header("Placement")]
  public SpawnEnemyOnDeath.SpawnPosition spawnPosition;
  [ShowInInspectorIf("ShowInsideColliderParams", true)]
  public int extraPixelWidth;
  [ShowInInspectorIf("ShowInsideRadiusParams", true)]
  public float spawnRadius = 1f;
  [Header("Spawn Parameters")]
  public float guaranteedSpawnGenerations;
  public string spawnAnim = "awaken";
  public bool spawnsCanDropLoot = true;
  public bool DoNormalReinforcement;
  private bool m_hasTriggered;

  private bool ShowRandomPrams() => this.enemySelection == SpawnEnemyOnDeath.EnemySelection.Random;

  private bool ShowInsideColliderParams()
  {
    return this.spawnPosition == SpawnEnemyOnDeath.SpawnPosition.InsideCollider;
  }

  private bool ShowInsideRadiusParams()
  {
    return this.spawnPosition == SpawnEnemyOnDeath.SpawnPosition.InsideRadius;
  }

  protected override void OnDestroy() => base.OnDestroy();

  protected override void OnTrigger(Vector2 damageDirection)
  {
    if (this.m_hasTriggered)
      return;
    this.m_hasTriggered = true;
    if ((double) this.guaranteedSpawnGenerations <= 0.0 && (double) this.chanceToSpawn < 1.0 && (double) UnityEngine.Random.value > (double) this.chanceToSpawn)
      return;
    if (!string.IsNullOrEmpty(this.spawnVfx))
      this.aiAnimator.PlayVfx(this.spawnVfx);
    string[] selectedEnemyGuids = (string[]) null;
    if (this.enemySelection == SpawnEnemyOnDeath.EnemySelection.All)
      selectedEnemyGuids = this.enemyGuidsToSpawn;
    else if (this.enemySelection == SpawnEnemyOnDeath.EnemySelection.Random)
    {
      selectedEnemyGuids = new string[UnityEngine.Random.Range(this.minSpawnCount, this.maxSpawnCount)];
      for (int index = 0; index < selectedEnemyGuids.Length; ++index)
        selectedEnemyGuids[index] = BraveUtility.RandomElement<string>(this.enemyGuidsToSpawn);
    }
    this.SpawnEnemies(selectedEnemyGuids);
  }

  public void ManuallyTrigger(Vector2 damageDirection) => this.OnTrigger(damageDirection);

  private void SpawnEnemies(string[] selectedEnemyGuids)
  {
    if (this.spawnPosition == SpawnEnemyOnDeath.SpawnPosition.InsideCollider)
    {
      IntVector2 intVector2_1 = this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
      if (this.aiActor.IsFalling || GameManager.Instance.Dungeon.CellIsPit(this.specRigidbody.UnitCenter.ToVector3ZUp()))
        return;
      RoomHandler roomFromPosition = GameManager.Instance.Dungeon.GetRoomFromPosition(intVector2_1);
      List<SpeculativeRigidbody> speculativeRigidbodyList = new List<SpeculativeRigidbody>();
      speculativeRigidbodyList.Add(this.specRigidbody);
      Vector2 unitBottomLeft = this.specRigidbody.UnitBottomLeft;
      for (int index = 0; index < selectedEnemyGuids.Length; ++index)
      {
        AIActor spawnedActor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(selectedEnemyGuids[index]), this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor), roomFromPosition);
        if (this.aiActor.IsBlackPhantom)
          spawnedActor.ForceBlackPhantom = true;
        if ((bool) (UnityEngine.Object) spawnedActor)
        {
          spawnedActor.specRigidbody.Initialize();
          Vector2 a = unitBottomLeft - (spawnedActor.specRigidbody.UnitBottomLeft - spawnedActor.transform.position.XY());
          Vector2 b = a + new Vector2(Mathf.Max(0.0f, this.specRigidbody.UnitDimensions.x - spawnedActor.specRigidbody.UnitDimensions.x), 0.0f);
          spawnedActor.transform.position = (Vector3) Vector2.Lerp(a, b, selectedEnemyGuids.Length != 1 ? (float) index / ((float) selectedEnemyGuids.Length - 1f) : 0.0f);
          spawnedActor.specRigidbody.Reinitialize();
          IntVector2 intVector2_2 = PhysicsEngine.UnitToPixel(Vector2.Lerp(a - new Vector2(PhysicsEngine.PixelToUnit(this.extraPixelWidth), 0.0f), b + new Vector2(PhysicsEngine.PixelToUnit(this.extraPixelWidth), 0.0f), selectedEnemyGuids.Length != 1 ? (float) index / ((float) selectedEnemyGuids.Length - 1f) : 0.5f) - spawnedActor.transform.position.XY());
          CollisionData result = (CollisionData) null;
          if (PhysicsEngine.Instance.RigidbodyCastWithIgnores(spawnedActor.specRigidbody, intVector2_2, out result, true, true, new int?(), false, speculativeRigidbodyList.ToArray()))
            intVector2_2 = result.NewPixelsToMove;
          CollisionData.Pool.Free(ref result);
          spawnedActor.transform.position += (Vector3) PhysicsEngine.PixelToUnit(intVector2_2);
          spawnedActor.specRigidbody.Reinitialize();
          if (index == 0)
            spawnedActor.aiAnimator.FacingDirection = 180f;
          else if (index == selectedEnemyGuids.Length - 1)
            spawnedActor.aiAnimator.FacingDirection = 0.0f;
          this.HandleSpawn(spawnedActor);
          speculativeRigidbodyList.Add(spawnedActor.specRigidbody);
        }
      }
      for (int index1 = 0; index1 < speculativeRigidbodyList.Count; ++index1)
      {
        for (int index2 = 0; index2 < speculativeRigidbodyList.Count; ++index2)
        {
          if (index1 != index2)
            speculativeRigidbodyList[index1].RegisterGhostCollisionException(speculativeRigidbodyList[index2]);
        }
      }
    }
    else if (this.spawnPosition == SpawnEnemyOnDeath.SpawnPosition.ScreenEdge)
    {
      for (int index3 = 0; index3 < selectedEnemyGuids.Length; ++index3)
      {
        AIActor spawnedActor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(selectedEnemyGuids[index3]), this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor), this.aiActor.ParentRoom);
        if ((bool) (UnityEngine.Object) spawnedActor)
        {
          Vector2 cameraBottomLeft = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(0.0f, 0.0f), ViewportType.Gameplay);
          Vector2 cameraTopRight = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(1f, 1f), ViewportType.Gameplay);
          IntVector2 bottomLeft = cameraBottomLeft.ToIntVector2(VectorConversions.Ceil);
          IntVector2 topRight = cameraTopRight.ToIntVector2(VectorConversions.Floor) - IntVector2.One;
          CellValidator cellValidator = (CellValidator) (c =>
          {
            for (int index4 = 0; index4 < spawnedActor.Clearance.x; ++index4)
            {
              for (int index5 = 0; index5 < spawnedActor.Clearance.y; ++index5)
              {
                if (GameManager.Instance.Dungeon.data.isTopWall(c.x + index4, c.y + index5) || GameManager.Instance.Dungeon.data[c.x + index4, c.y + index5].isExitCell)
                  return false;
              }
            }
            return c.x >= bottomLeft.x && c.y >= bottomLeft.y && c.x + spawnedActor.Clearance.x - 1 <= topRight.x && c.y + spawnedActor.Clearance.y - 1 <= topRight.y;
          });
          Func<IntVector2, float> cellWeightFinder = (Func<IntVector2, float>) (c => Mathf.Min(Mathf.Min(Mathf.Min(Mathf.Min(float.MaxValue, (float) c.x - cameraBottomLeft.x), (float) c.y - cameraBottomLeft.y), cameraTopRight.x - (float) c.x + (float) spawnedActor.Clearance.x), cameraTopRight.y - (float) c.y + (float) spawnedActor.Clearance.y));
          Vector2 vector2 = spawnedActor.specRigidbody.UnitCenter - spawnedActor.transform.position.XY();
          IntVector2? weightedAvailableCell = spawnedActor.ParentRoom.GetRandomWeightedAvailableCell(new IntVector2?(spawnedActor.Clearance), new CellTypes?(spawnedActor.PathableTiles), cellValidator: cellValidator, cellWeightFinder: cellWeightFinder, percentageBounds: 0.25f);
          if (!weightedAvailableCell.HasValue)
          {
            Debug.LogError((object) "Screen Edge Spawn FAILED!", (UnityEngine.Object) spawnedActor);
            UnityEngine.Object.Destroy((UnityEngine.Object) spawnedActor);
          }
          else
          {
            spawnedActor.transform.position = (Vector3) (Pathfinder.GetClearanceOffset(weightedAvailableCell.Value, spawnedActor.Clearance) - vector2);
            spawnedActor.specRigidbody.Reinitialize();
            this.HandleSpawn(spawnedActor);
          }
        }
      }
    }
    else if (this.spawnPosition == SpawnEnemyOnDeath.SpawnPosition.InsideRadius)
    {
      Vector2 unitCenter = this.specRigidbody.GetUnitCenter(ColliderType.HitBox);
      List<SpeculativeRigidbody> speculativeRigidbodyList = new List<SpeculativeRigidbody>();
      speculativeRigidbodyList.Add(this.specRigidbody);
      for (int index = 0; index < selectedEnemyGuids.Length; ++index)
      {
        Vector2 rhs = unitCenter + UnityEngine.Random.insideUnitCircle * this.spawnRadius;
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST && SceneManager.GetActiveScene().name == "fs_robot")
        {
          RoomHandler entrance = GameManager.Instance.Dungeon.data.Entrance;
          Vector2 lhs = entrance.area.basePosition.ToVector2() + new Vector2(7f, 7f);
          rhs = Vector2.Min(entrance.area.basePosition.ToVector2() + new Vector2(38f, 36f), Vector2.Max(lhs, rhs));
        }
        AIActor spawnedActor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(selectedEnemyGuids[index]), unitCenter.ToIntVector2(VectorConversions.Floor), this.aiActor.ParentRoom, true);
        if ((bool) (UnityEngine.Object) spawnedActor)
        {
          spawnedActor.specRigidbody.Initialize();
          Vector2 unit = rhs - spawnedActor.specRigidbody.GetUnitCenter(ColliderType.HitBox);
          spawnedActor.specRigidbody.ImpartedPixelsToMove = PhysicsEngine.UnitToPixel(unit);
          this.HandleSpawn(spawnedActor);
          speculativeRigidbodyList.Add(spawnedActor.specRigidbody);
        }
      }
      for (int index6 = 0; index6 < speculativeRigidbodyList.Count; ++index6)
      {
        for (int index7 = 0; index7 < speculativeRigidbodyList.Count; ++index7)
        {
          if (index6 != index7)
            speculativeRigidbodyList[index6].RegisterGhostCollisionException(speculativeRigidbodyList[index7]);
        }
      }
    }
    else
      Debug.LogError((object) ("Unknown spawn type: " + (object) this.spawnPosition));
  }

  private void HandleSpawn(AIActor spawnedActor)
  {
    if (!string.IsNullOrEmpty(this.spawnAnim))
      spawnedActor.aiAnimator.PlayUntilFinished(this.spawnAnim);
    SpawnEnemyOnDeath component = spawnedActor.GetComponent<SpawnEnemyOnDeath>();
    if ((bool) (UnityEngine.Object) component)
      component.guaranteedSpawnGenerations = Mathf.Max(0.0f, this.guaranteedSpawnGenerations - 1f);
    if (!this.spawnsCanDropLoot)
    {
      spawnedActor.CanDropCurrency = false;
      spawnedActor.CanDropItems = false;
    }
    if (!this.DoNormalReinforcement)
      return;
    spawnedActor.HandleReinforcementFallIntoRoom(0.1f);
  }

  public enum EnemySelection
  {
    All = 10, // 0x0000000A
    Random = 20, // 0x00000014
  }

  public enum SpawnPosition
  {
    InsideCollider = 0,
    ScreenEdge = 1,
    InsideRadius = 20, // 0x00000014
  }
}
