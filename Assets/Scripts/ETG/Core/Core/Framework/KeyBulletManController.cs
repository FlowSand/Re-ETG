using Dungeonator;
using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable

public class KeyBulletManController : BraveBehaviour
  {
    [PickupIdentifier]
    [FormerlySerializedAs("keyId")]
    public int lookPickupId = -1;
    public GenericLootTable lootTable;
    public Vector2 offset;
    public bool doubleForBlackPhantom = true;
    public bool RemoveShaderOnDeath;
    private bool m_cachedIsBlackPhantom;

    public void Start()
    {
      this.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
      this.aiActor.OnHandleRewards += new System.Action(this.OnHandleRewards);
      this.aiActor.SuppressBlackPhantomCorpseBurn = true;
    }

    protected override void OnDestroy()
    {
      if ((bool) (UnityEngine.Object) this.healthHaver)
        this.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnPreDeath);
      if ((bool) (UnityEngine.Object) this.aiActor)
        this.aiActor.OnHandleRewards -= new System.Action(this.OnHandleRewards);
      base.OnDestroy();
    }

    private void OnPreDeath(Vector2 dir)
    {
      this.m_cachedIsBlackPhantom = this.aiActor.IsBlackPhantom;
      if (this.lookPickupId == GlobalItemIds.Key && this.aiActor.IsBlackPhantom)
        this.aiActor.UnbecomeBlackPhantom();
      if (!this.RemoveShaderOnDeath)
        return;
      this.renderer.sharedMaterials = new Material[1]
      {
        this.renderer.sharedMaterials[0]
      };
    }

    public void ForceHandleRewards() => this.OnHandleRewards();

    private void OnHandleRewards()
    {
      bool flag = false;
      if (this.lookPickupId == GlobalItemIds.Key)
      {
        GameStatsManager.Instance.RegisterStatChange(TrackedStats.KEYBULLETMEN_KILLED, 1f);
        flag = true;
      }
      Vector3 spawnPosition = this.transform.position + (Vector3) this.offset;
      if (!flag && GameManager.Instance.Dungeon.data.isAnyFaceWall(Mathf.FloorToInt(spawnPosition.x), Mathf.FloorToInt(spawnPosition.y + 0.5f)))
        spawnPosition += new Vector3(0.0f, -1f, 0.0f);
      CellData cell1 = (spawnPosition + new Vector3(0.0f, 0.5f, 0.0f)).GetCell();
      if (cell1 == null || cell1.type == CellType.WALL || cell1.IsAnyFaceWall())
      {
        CellData cell2 = (spawnPosition += Vector3.down).GetCell();
        if (cell2 != null && cell2.type != CellType.WALL)
          spawnPosition += Vector3.down;
      }
      if (this.doubleForBlackPhantom && this.m_cachedIsBlackPhantom)
      {
        LootEngine.SpawnItem(this.GetReward(), spawnPosition, Vector2.left, 2f, false, disableHeightBoost: true);
        LootEngine.SpawnItem(this.GetReward(), spawnPosition, Vector2.right, 2f, false, disableHeightBoost: true);
      }
      else if (flag)
        LootEngine.SpawnItem(this.GetReward(), spawnPosition, Vector2.zero, 0.0f, disableHeightBoost: true);
      else
        LootEngine.SpawnItem(this.GetReward(), spawnPosition, Vector2.up, 0.1f, disableHeightBoost: true);
    }

    private GameObject GetReward()
    {
      return (bool) (UnityEngine.Object) this.lootTable ? this.lootTable.SelectByWeight() : PickupObjectDatabase.GetById(this.lookPickupId).gameObject;
    }
  }

