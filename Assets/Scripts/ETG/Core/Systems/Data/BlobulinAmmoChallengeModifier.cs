using Dungeonator;
using System;

#nullable disable

public class BlobulinAmmoChallengeModifier : ChallengeModifier
  {
    [EnemyIdentifier]
    public string SpawnTargetGuid;
    public float CooldownBetweenSpawns = 0.2f;
    private float m_cooldown;
    public float SafeRadius = 3f;

    private void Start()
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        GameManager.Instance.AllPlayers[index].PostProcessProjectile += new Action<Projectile, float>(this.ModifyProjectile);
    }

    private void ModifyProjectile(Projectile proj, float somethin)
    {
      if (!(bool) (UnityEngine.Object) proj || !(proj.Owner is PlayerController) || proj.SpawnedFromNonChallengeItem || proj.TreatedAsNonProjectileForChallenge)
        return;
      switch (proj)
      {
        case InstantDamageOneEnemyProjectile _:
          break;
        case InstantlyDamageAllProjectile _:
          break;
        default:
          proj.OnDestruction += new Action<Projectile>(this.HandleProjectileDeath);
          break;
      }
    }

    private bool CellIsValid(IntVector2 cellPos)
    {
      if (!GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(cellPos))
        return false;
      CellData cellData = GameManager.Instance.Dungeon.data[cellPos];
      return (cellData == null || cellData.parentRoom == null || cellData.parentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.RoomClear) != 0) && cellData != null && cellData.type == CellType.FLOOR && cellData.IsPassable && cellData.parentRoom == GameManager.Instance.BestActivePlayer.CurrentRoom && !cellData.isExitCell;
    }

    private void Update() => this.m_cooldown -= BraveTime.DeltaTime;

    private void HandleProjectileDeath(Projectile obj)
    {
      if (!(bool) (UnityEngine.Object) this || !(bool) (UnityEngine.Object) obj || obj.HasImpactedEnemy || obj.HasDiedInAir)
        return;
      float range = 0.0f;
      GameManager.Instance.GetPlayerClosestToPoint(obj.specRigidbody.UnitCenter, out range);
      if ((double) range < (double) this.SafeRadius)
        return;
      IntVector2 intVector2 = obj.specRigidbody.UnitCenter.ToIntVector2();
      if (GameManager.Instance.Dungeon.data.isFaceWallHigher(intVector2.x, intVector2.y))
        intVector2 += new IntVector2(0, -2);
      else if (GameManager.Instance.Dungeon.data.isFaceWallLower(intVector2.x, intVector2.y))
        intVector2 += new IntVector2(0, -1);
      bool flag = this.CellIsValid(intVector2);
      if (!flag)
      {
        for (int x = -1; x < 2; ++x)
        {
          for (int y = -1; y < 2; ++y)
          {
            IntVector2 cellPos = intVector2 + new IntVector2(x, y);
            flag = this.CellIsValid(cellPos);
            if (flag)
            {
              intVector2 = cellPos;
              break;
            }
          }
          if (flag)
            break;
        }
        if (!flag)
        {
          IntVector2? nearestAvailableCell = GameManager.Instance.PrimaryPlayer.CurrentRoom.GetNearestAvailableCell(obj.specRigidbody.UnitCenter, new IntVector2?(IntVector2.One), new CellTypes?(CellTypes.FLOOR));
          if (nearestAvailableCell.HasValue)
          {
            flag = true;
            intVector2 = nearestAvailableCell.Value;
          }
        }
      }
      if (obj.Owner is PlayerController)
      {
        if (!(obj.Owner as PlayerController).IsInCombat)
          flag = false;
      }
      else
        flag = false;
      if (!flag || (double) this.m_cooldown > 0.0)
        return;
      this.m_cooldown = this.CooldownBetweenSpawns;
      AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(this.SpawnTargetGuid), intVector2, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector2), true);
    }

    private void OnDestroy()
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        GameManager.Instance.AllPlayers[index].PostProcessProjectile -= new Action<Projectile, float>(this.ModifyProjectile);
    }
  }

