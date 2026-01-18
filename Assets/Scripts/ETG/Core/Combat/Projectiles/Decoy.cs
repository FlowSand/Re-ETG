using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class Decoy : SpawnObjectItem
  {
    public string revealAnimationName;
    public GameObject revealVFX;
    public bool ExplodesOnDeath;
    public float DeathExplosionTimer = -1f;
    public ExplosionData DeathExplosion;
    public bool AllowStealing = true;
    [Header("Synergues")]
    public bool HasGoopSynergy;
    public CustomSynergyType GoopSynergy;
    public GoopDefinition GoopSynergyGoop;
    public float GoopSynergyRadius;
    public string GoopSynergySprite;
    public bool HasFreezeAttackersSynergy;
    public CustomSynergyType FreezeAttackersSynergy;
    public GameActorFreezeEffect FreezeSynergyEffect;
    public string FreezeAttackersSprite;
    public bool HasDecoyOctopusSynergy;
    public GameActorCharmEffect PermanentCharmEffect;
    private bool m_revealed;

    [DebuggerHidden]
    private IEnumerator Start()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Decoy__Startc__Iterator0()
      {
        _this = this
      };
    }

    private void HandlePreRigidbodyCollision(
      SpeculativeRigidbody myRigidbody,
      PixelCollider myPixelCollider,
      SpeculativeRigidbody otherRigidbody,
      PixelCollider otherPixelCollider)
    {
      if (this.m_revealed)
      {
        PhysicsEngine.SkipCollision = true;
      }
      else
      {
        if (this.HasFreezeAttackersSynergy && (bool) (Object) this.SpawningPlayer && this.SpawningPlayer.HasActiveBonusSynergy(this.FreezeAttackersSynergy) && (bool) (Object) otherRigidbody && (bool) (Object) otherRigidbody.projectile)
        {
          Projectile projectile = otherRigidbody.projectile;
          if (projectile.Owner is AIActor)
            (projectile.Owner as AIActor).ApplyEffect((GameActorEffect) this.FreezeSynergyEffect);
          else if ((bool) (Object) projectile.Shooter && (bool) (Object) projectile.Shooter.aiActor)
            projectile.Shooter.aiActor.ApplyEffect((GameActorEffect) this.FreezeSynergyEffect);
        }
        if (!this.HasDecoyOctopusSynergy || !(bool) (Object) this.SpawningPlayer || !this.SpawningPlayer.HasActiveBonusSynergy(CustomSynergyType.DECOY_OCTOPUS) || !(bool) (Object) otherRigidbody || !(bool) (Object) otherRigidbody.projectile)
          return;
        Projectile projectile1 = otherRigidbody.projectile;
        string guid = string.Empty;
        if (projectile1.Owner is AIActor)
        {
          AIActor owner = projectile1.Owner as AIActor;
          if (owner.IsNormalEnemy && (bool) (Object) owner.healthHaver && !owner.healthHaver.IsBoss)
            guid = owner.EnemyGuid;
        }
        else if ((bool) (Object) projectile1.Shooter && (bool) (Object) projectile1.Shooter.aiActor)
        {
          AIActor aiActor = projectile1.Shooter.aiActor;
          if (aiActor.IsNormalEnemy && (bool) (Object) aiActor.healthHaver && !aiActor.healthHaver.IsBoss)
            guid = aiActor.EnemyGuid;
        }
        if (string.IsNullOrEmpty(guid))
          return;
        this.OnBreak();
        AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(guid), this.transform.position.IntXY(VectorConversions.Floor), this.transform.position.GetAbsoluteRoom(), true).ApplyEffect((GameActorEffect) this.PermanentCharmEffect);
        projectile1.DieInAir();
        PhysicsEngine.SkipCollision = true;
      }
    }

    private void OnBreak()
    {
      if (!this.m_revealed)
      {
        this.m_revealed = true;
        if ((Object) this.revealVFX != (Object) null)
          this.revealVFX.SetActive(true);
        if (this.ExplodesOnDeath)
        {
          if ((double) this.DeathExplosion.damageToPlayer > 0.0)
            this.DeathExplosion.damageToPlayer = 0.0f;
          Exploder.Explode((Vector3) this.specRigidbody.UnitCenter, this.DeathExplosion, Vector2.zero);
          Object.Destroy((Object) this.gameObject);
        }
        else
          this.spriteAnimator.PlayAndDestroyObject(this.revealAnimationName);
      }
      List<BaseShopController> allShops = StaticReferenceManager.AllShops;
      RoomHandler roomFromPosition = GameManager.Instance.Dungeon.GetRoomFromPosition(this.transform.position.IntXY(VectorConversions.Floor));
      for (int index = 0; index < allShops.Count; ++index)
      {
        if ((bool) (Object) allShops[index] && allShops[index].GetAbsoluteParentRoom() == roomFromPosition)
          allShops[index].SetCapableOfBeingStolenFrom(false, nameof (Decoy));
      }
      if (!this.HasGoopSynergy || !(bool) (Object) this.SpawningPlayer || !this.SpawningPlayer.HasActiveBonusSynergy(this.GoopSynergy))
        return;
      DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.GoopSynergyGoop).TimedAddGoopCircle(this.specRigidbody.UnitCenter, this.GoopSynergyRadius, 1f);
    }

    private void ClearOverrides(RoomHandler room)
    {
      List<AIActor> activeEnemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
      if (activeEnemies == null)
        return;
      for (int index = 0; index < activeEnemies.Count; ++index)
      {
        if ((Object) activeEnemies[index].OverrideTarget == (Object) this.specRigidbody)
          activeEnemies[index].OverrideTarget = (SpeculativeRigidbody) null;
      }
    }

    private void AttractEnemies(RoomHandler room)
    {
      List<AIActor> activeEnemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
      if (activeEnemies == null)
        return;
      for (int index = 0; index < activeEnemies.Count; ++index)
      {
        if ((Object) activeEnemies[index].OverrideTarget == (Object) null)
          activeEnemies[index].OverrideTarget = this.specRigidbody;
      }
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

