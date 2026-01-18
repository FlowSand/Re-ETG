// Decompiled with JetBrains decompiler
// Type: CompanionController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class CompanionController : BraveBehaviour
  {
    public bool CanInterceptBullets;
    public bool IsCopDead;
    public bool IsCop;
    public StatModifier CopDeathStatModifier;
    public int CurseOnCopDeath = 2;
    public bool CanCrossPits;
    public bool BlanksOnActiveItemUsed;
    public float InternalBlankCooldown = 10f;
    public bool HasStealthMode;
    public bool PredictsChests;
    [LongNumericEnum]
    public CustomSynergyType PredictsChestSynergy;
    public bool CanBePet;
    public CompanionController.CompanionIdentifier companionID;
    public HeatRingModule TeaSynergyHeatRing;
    protected PlayerController m_owner;
    protected Chest m_lastPredictedChest;
    protected HologramDoer m_hologram;
    protected float m_internalBlankCooldown;
    protected CellType m_lastCellType = CellType.FLOOR;
    protected Vector2 m_cachedRollDirection;
    protected bool m_isStealthed;
    protected float m_timeInDeadlyRoom;
    private bool m_hasDoneJunkanCheck;
    private bool m_hasAttemptedSynergy;
    private bool m_hasLuteBuff;
    private GameObject m_luteOverheadVfx;
    private bool m_isMimicTransforming;
    public PlayerController m_pettingDoer;
    public Vector2 m_petOffset;

    [DebuggerHidden]
    private IEnumerator HandleDelayedInitialization()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new CompanionController__HandleDelayedInitializationc__Iterator0()
      {
        _this = this
      };
    }

    public void Initialize(PlayerController owner)
    {
      this.m_owner = owner;
      this.gameActor.ImmuneToAllEffects = true;
      this.aiActor.SetResistance(EffectResistanceType.Poison, 1f);
      this.aiActor.SetResistance(EffectResistanceType.Fire, 1f);
      this.aiActor.SetResistance(EffectResistanceType.Freeze, 1f);
      this.aiActor.SetResistance(EffectResistanceType.Charm, 1f);
      this.aiActor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.PlayerCollider));
      if (this.companionID == CompanionController.CompanionIdentifier.GATLING_GULL)
        this.aiActor.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyCollider));
      this.aiActor.IsNormalEnemy = false;
      this.aiActor.CompanionOwner = this.m_owner;
      this.aiActor.CanTargetPlayers = false;
      this.aiActor.CanTargetEnemies = true;
      this.aiActor.CustomPitDeathHandling += new AIActor.CustomPitHandlingDelegate(this.CustomPitDeathHandling);
      this.aiActor.State = AIActor.ActorState.Normal;
      this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision);
      if ((bool) (UnityEngine.Object) this.bulletBank)
        this.bulletBank.OnProjectileCreated += new Action<Projectile>(this.MarkNondamaging);
      if (!this.CanInterceptBullets)
      {
        this.specRigidbody.HitboxPixelCollider.IsTrigger = true;
        this.specRigidbody.HitboxPixelCollider.CollisionLayerIgnoreOverride |= CollisionMask.LayerToMask(CollisionLayer.Projectile);
      }
      if (this.IsCop)
      {
        this.healthHaver.ManualDeathHandling = true;
        this.healthHaver.OnPreDeath += new Action<Vector2>(this.HandleCopDeath);
        this.healthHaver.ModifyDamage += new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.HandleCopModifyDamage);
      }
      if (this.PredictsChests)
        this.m_hologram = this.GetComponentInChildren<HologramDoer>();
      if ((bool) (UnityEngine.Object) this.bulletBank)
        this.bulletBank.OnProjectileCreated += new Action<Projectile>(this.HandleCompanionPostProcessProjectile);
      if ((bool) (UnityEngine.Object) this.aiShooter)
        this.aiShooter.PostProcessProjectile += new Action<Projectile>(this.HandleCompanionPostProcessProjectile);
      if (this.BlanksOnActiveItemUsed)
        owner.OnUsedPlayerItem += new Action<PlayerController, PlayerItem>(this.HandleItemUsed);
      owner.OnPitfall += new System.Action(this.HandlePitfall);
      owner.OnRoomClearEvent += new Action<PlayerController>(this.HandleRoomCleared);
      owner.companions.Add(this.aiActor);
      this.StartCoroutine(this.HandleDelayedInitialization());
    }

    private void HandleCopModifyDamage(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
    {
      if (args == EventArgs.Empty || !(bool) (UnityEngine.Object) this.m_owner || !this.m_owner.HasActiveBonusSynergy(CustomSynergyType.COP_VEST))
        return;
      args.ModifiedDamage /= 2f;
    }

    protected virtual void HandleRoomCleared(PlayerController callingPlayer)
    {
    }

    protected void MarkNondamaging(Projectile obj)
    {
      if (!(bool) (UnityEngine.Object) obj)
        return;
      obj.collidesWithPlayer = false;
    }

    protected void HandlePitfall()
    {
    }

    protected void HandleCompanionPostProcessProjectile(Projectile obj)
    {
      if ((bool) (UnityEngine.Object) obj)
      {
        obj.collidesWithPlayer = false;
        obj.TreatedAsNonProjectileForChallenge = true;
      }
      if (!(bool) (UnityEngine.Object) this.m_owner)
        return;
      if (PassiveItem.IsFlagSetForCharacter(this.m_owner, typeof (BattleStandardItem)))
        obj.baseData.damage *= BattleStandardItem.BattleStandardCompanionDamageMultiplier;
      if ((bool) (UnityEngine.Object) this.m_owner.CurrentGun && this.m_owner.CurrentGun.LuteCompanionBuffActive)
      {
        obj.baseData.damage *= 2f;
        obj.RuntimeUpdateScale(1f / obj.AdditionalScaleMultiplier);
        obj.RuntimeUpdateScale(1.75f);
      }
      this.m_owner.DoPostProcessProjectile(obj);
    }

    protected void HandleItemUsed(PlayerController arg1, PlayerItem arg2)
    {
      if (arg1.HasActiveBonusSynergy(CustomSynergyType.ELDER_AND_YOUNGER))
      {
        GameManager.Instance.Dungeon.StartCoroutine(this.HandleDelayedBlank(arg1));
      }
      else
      {
        if ((double) this.m_internalBlankCooldown > 0.0)
          return;
        arg1.ForceBlank(overrideCenter: new Vector2?(this.sprite.WorldCenter));
        this.m_internalBlankCooldown = this.InternalBlankCooldown;
      }
    }

    [DebuggerHidden]
    private IEnumerator HandleDelayedBlank(PlayerController arg1)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new CompanionController__HandleDelayedBlankc__Iterator1()
      {
        arg1 = arg1,
        _this = this
      };
    }

    protected void HandleCopDeath(Vector2 obj) => this.StartCoroutine(this.HandleCopDeath_CR());

    public virtual void Update()
    {
      if (!(bool) (UnityEngine.Object) GameManager.Instance || GameManager.Instance.IsLoadingLevel)
        return;
      if (this.IsBeingPet && (!(bool) (UnityEngine.Object) this.m_pettingDoer || (UnityEngine.Object) this.m_pettingDoer.m_pettingTarget != (UnityEngine.Object) this || !this.aiAnimator.IsPlaying("pet") || (double) Vector2.Distance(this.specRigidbody.UnitCenter, this.m_pettingDoer.specRigidbody.UnitCenter) > 3.0))
        this.StopPet();
      if (!this.m_hasDoneJunkanCheck)
      {
        if (this.m_owner.companions.Count >= 2)
        {
          int num = 0;
          for (int index = 0; index < this.m_owner.companions.Count; ++index)
          {
            if ((bool) (UnityEngine.Object) this.m_owner.companions[index])
              ++num;
          }
          if (num >= 2)
            GameStatsManager.Instance.SetFlag(GungeonFlags.ITEMSPECIFIC_SER_JUNKAN_MAXLVL, true);
        }
        this.m_hasDoneJunkanCheck = true;
      }
      if ((double) this.m_internalBlankCooldown > 0.0)
        this.m_internalBlankCooldown -= BraveTime.DeltaTime;
      if (this.BlanksOnActiveItemUsed && (bool) (UnityEngine.Object) this.m_owner && this.m_owner.HasActiveBonusSynergy(CustomSynergyType.MY_LITTLE_FRIEND))
      {
        if (!this.m_hasAttemptedSynergy && (bool) (UnityEngine.Object) this.m_owner.CurrentGun && this.m_owner.CurrentGun.ClipShotsRemaining == 0)
        {
          this.m_hasAttemptedSynergy = true;
          if ((double) UnityEngine.Random.value < 0.25)
            this.HandleItemUsed(this.m_owner, (PlayerItem) null);
        }
        else if (this.m_hasAttemptedSynergy && (bool) (UnityEngine.Object) this.m_owner.CurrentGun && this.m_owner.CurrentGun.ClipShotsRemaining != 0)
          this.m_hasAttemptedSynergy = false;
      }
      if (this.companionID == CompanionController.CompanionIdentifier.SUPER_SPACE_TURTLE && (bool) (UnityEngine.Object) this.m_owner && this.m_owner.HasActiveBonusSynergy(CustomSynergyType.OUTER_TURTLE) && !this.aiActor.IsBlackPhantom)
        this.aiActor.BecomeBlackPhantom();
      if (this.HasStealthMode && (bool) (UnityEngine.Object) this.m_owner)
      {
        if (this.m_owner.IsStealthed && !this.m_isStealthed)
        {
          this.m_isStealthed = true;
          this.aiAnimator.IdleAnimation.AnimNames[0] = "sst_dis_idle_right";
          this.aiAnimator.IdleAnimation.AnimNames[1] = "sst_dis_idle_left";
          this.aiAnimator.MoveAnimation.AnimNames[0] = "sst_dis_move_right";
          this.aiAnimator.MoveAnimation.AnimNames[1] = "sst_dis_move_left";
        }
        else if (!this.m_owner.IsStealthed && this.m_isStealthed)
        {
          this.m_isStealthed = false;
          this.aiAnimator.IdleAnimation.AnimNames[0] = "sst_idle_right";
          this.aiAnimator.IdleAnimation.AnimNames[1] = "sst_idle_left";
          this.aiAnimator.MoveAnimation.AnimNames[0] = "sst_move_right";
          this.aiAnimator.MoveAnimation.AnimNames[1] = "sst_move_left";
        }
      }
      if ((bool) (UnityEngine.Object) this.m_owner && (bool) (UnityEngine.Object) this.sprite && (bool) (UnityEngine.Object) this.m_owner.CurrentGun && this.m_owner.CurrentGun.IsReloading && this.m_owner.HasActiveBonusSynergy(CustomSynergyType.TEA_FOR_TWO))
      {
        AuraOnReloadModifier component = this.m_owner.CurrentGun.GetComponent<AuraOnReloadModifier>();
        if (this.TeaSynergyHeatRing == null)
          this.TeaSynergyHeatRing = new HeatRingModule();
        if (this.TeaSynergyHeatRing != null && !this.TeaSynergyHeatRing.IsActive && (bool) (UnityEngine.Object) component && component.IgnitesEnemies && (bool) (UnityEngine.Object) this.m_owner && (bool) (UnityEngine.Object) this.m_owner.CurrentGun && (bool) (UnityEngine.Object) this.sprite)
          this.TeaSynergyHeatRing.Trigger(component.AuraRadius, this.m_owner.CurrentGun.reloadTime, component.IgniteEffect, this.sprite);
      }
      if ((bool) (UnityEngine.Object) this.m_owner && this.companionID == CompanionController.CompanionIdentifier.SHELLETON)
      {
        ((BasicAttackBehavior) this.behaviorSpeculator.AttackBehaviors[1]).IsBlackPhantom = !this.m_owner.HasActiveBonusSynergy(CustomSynergyType.BIRTHRIGHT);
        this.behaviorSpeculator.LocalTimeScale = !this.m_owner.HasActiveBonusSynergy(CustomSynergyType.SHELL_A_TON) ? 1f : 2f;
      }
      if (this.IsCop && GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.FORGEGEON)
      {
        CellData cell = this.transform.position.GetCell();
        if (this.transform.position.GetAbsoluteRoom().area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS)
          this.healthHaver.ApplyDamage(1000000f, Vector2.zero, "Inevitability", damageCategory: DamageCategory.Unstoppable);
        else if (this.transform.position.GetAbsoluteRoom().HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear) && this.m_owner.CurrentRoom.distanceFromEntrance > 1 && (cell == null || !cell.isExitCell))
        {
          if ((double) this.m_timeInDeadlyRoom > 5.0 && (double) Vector2.Distance(this.m_owner.CenterPosition, this.transform.position.XY()) < 12.0)
            this.healthHaver.ApplyDamage(1000000f, Vector2.zero, "Inevitability", damageCategory: DamageCategory.Unstoppable);
          else
            this.m_timeInDeadlyRoom += BraveTime.DeltaTime;
        }
        else
          this.m_timeInDeadlyRoom = 0.0f;
      }
      IntVector2 intVector2 = this.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor);
      if (GameManager.Instance.Dungeon.data.CheckInBounds(intVector2))
      {
        CellData cellData = GameManager.Instance.Dungeon.data[intVector2];
        if (cellData != null)
          this.m_lastCellType = cellData.type;
      }
      if (this.PredictsChests && (bool) (UnityEngine.Object) this.m_owner && this.m_owner.HasActiveBonusSynergy(this.PredictsChestSynergy))
      {
        Chest chest = (Chest) null;
        float num1 = float.MaxValue;
        for (int index = 0; index < StaticReferenceManager.AllChests.Count; ++index)
        {
          Chest allChest = StaticReferenceManager.AllChests[index];
          if ((bool) (UnityEngine.Object) allChest && (bool) (UnityEngine.Object) allChest.sprite && !allChest.IsOpen && !allChest.IsBroken && !allChest.IsSealed)
          {
            float num2 = Vector2.Distance(this.m_owner.CenterPosition, allChest.sprite.WorldCenter);
            if ((double) num2 < (double) num1)
            {
              chest = allChest;
              num1 = num2;
            }
          }
        }
        if ((double) num1 > 5.0)
          chest = (Chest) null;
        if ((UnityEngine.Object) this.m_lastPredictedChest != (UnityEngine.Object) chest)
        {
          if ((bool) (UnityEngine.Object) this.m_lastPredictedChest)
            this.m_hologram.HideSprite(this.gameObject);
          if ((bool) (UnityEngine.Object) chest)
          {
            List<PickupObject> pickupObjectList = chest.PredictContents(this.m_owner);
            if (pickupObjectList.Count > 0 && (bool) (UnityEngine.Object) pickupObjectList[0].encounterTrackable)
            {
              tk2dSpriteCollectionData encounterIconCollection = AmmonomiconController.ForceInstance.EncounterIconCollection;
              this.m_hologram.ChangeToSprite(this.gameObject, encounterIconCollection, encounterIconCollection.GetSpriteIdByName(pickupObjectList[0].encounterTrackable.journalData.AmmonomiconSprite));
            }
          }
          this.m_lastPredictedChest = chest;
        }
      }
      else if ((bool) (UnityEngine.Object) this.m_hologram && this.m_hologram.ArcRenderer.enabled)
        this.m_hologram.HideSprite(this.gameObject, true);
      if (this.companionID == CompanionController.CompanionIdentifier.BABY_GOOD_MIMIC && !this.m_isMimicTransforming)
        this.HandleBabyGoodMimic();
      if (!(bool) (UnityEngine.Object) this.m_owner || !(bool) (UnityEngine.Object) this.m_owner.CurrentGun || !(bool) (UnityEngine.Object) this.aiActor)
        return;
      if (this.m_hasLuteBuff && !this.m_owner.CurrentGun.LuteCompanionBuffActive)
      {
        if ((bool) (UnityEngine.Object) this.m_luteOverheadVfx)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_luteOverheadVfx);
          this.m_luteOverheadVfx = (GameObject) null;
        }
        this.m_hasLuteBuff = false;
      }
      else
      {
        if (this.m_hasLuteBuff || !this.m_owner.CurrentGun.LuteCompanionBuffActive)
          return;
        this.m_luteOverheadVfx = this.aiActor.PlayEffectOnActor((GameObject) ResourceCache.Acquire("Global VFX/VFX_Buff_Status"), new Vector3(0.0f, 1.25f, 0.0f), alreadyMiddleCenter: true);
        this.m_hasLuteBuff = true;
      }
    }

    protected void HandleBabyGoodMimic()
    {
      if (!(bool) (UnityEngine.Object) this.m_owner)
        return;
      CompanionItem sourceItem = (CompanionItem) null;
      string targetGuid = string.Empty;
      for (int index = 0; index < this.m_owner.passiveItems.Count; ++index)
      {
        if (this.m_owner.passiveItems[index] is CompanionItem)
        {
          sourceItem = this.m_owner.passiveItems[index] as CompanionItem;
          if ((UnityEngine.Object) sourceItem.ExtantCompanion != (UnityEngine.Object) this.gameObject)
            sourceItem = (CompanionItem) null;
          else
            break;
        }
      }
      for (int index = 0; index < this.m_owner.companions.Count; ++index)
      {
        CompanionController component = this.m_owner.companions[index].GetComponent<CompanionController>();
        if ((!(bool) (UnityEngine.Object) component || component.companionID != CompanionController.CompanionIdentifier.GATLING_GULL) && (!(bool) (UnityEngine.Object) component || component.companionID != CompanionController.CompanionIdentifier.BABY_GOOD_MIMIC))
        {
          targetGuid = this.m_owner.companions[index].EnemyGuid;
          break;
        }
      }
      PlayerOrbitalItem orbitalItemTarget = (PlayerOrbitalItem) null;
      if (string.IsNullOrEmpty(targetGuid))
      {
        for (int index = 0; index < this.m_owner.passiveItems.Count; ++index)
        {
          if (this.m_owner.passiveItems[index] is PlayerOrbitalItem)
          {
            PlayerOrbitalItem passiveItem = this.m_owner.passiveItems[index] as PlayerOrbitalItem;
            if (passiveItem.CanBeMimicked)
            {
              orbitalItemTarget = passiveItem;
              break;
            }
          }
        }
      }
      if (!(bool) (UnityEngine.Object) sourceItem || string.IsNullOrEmpty(targetGuid) && !((UnityEngine.Object) orbitalItemTarget != (UnityEngine.Object) null))
        return;
      this.StartCoroutine(this.HandleBabyMimicTransform(sourceItem, targetGuid, orbitalItemTarget));
    }

    [DebuggerHidden]
    private IEnumerator HandleBabyMimicTransform(
      CompanionItem sourceItem,
      string targetGuid,
      PlayerOrbitalItem orbitalItemTarget = null)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new CompanionController__HandleBabyMimicTransformc__Iterator2()
      {
        sourceItem = sourceItem,
        targetGuid = targetGuid,
        orbitalItemTarget = orbitalItemTarget,
        _this = this
      };
    }

    protected bool PlayerRoomHasActiveEnemies()
    {
      bool flag = this.transform.position.GetAbsoluteRoom().HasActiveEnemies(RoomHandler.ActiveEnemyType.All);
      if (!flag)
        flag = GameManager.Instance.PrimaryPlayer.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All);
      return flag;
    }

    [DebuggerHidden]
    private IEnumerator HandleCopDeath_CR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new CompanionController__HandleCopDeath_CRc__Iterator3()
      {
        _this = this
      };
    }

    protected void HandlePreCollision(
      SpeculativeRigidbody myRigidbody,
      PixelCollider myPixelCollider,
      SpeculativeRigidbody otherRigidbody,
      PixelCollider otherPixelCollider)
    {
      if ((UnityEngine.Object) otherRigidbody.transform.parent != (UnityEngine.Object) null && ((bool) (UnityEngine.Object) otherRigidbody.transform.parent.GetComponent<DungeonDoorController>() || (bool) (UnityEngine.Object) otherRigidbody.transform.parent.GetComponent<DungeonDoorSubsidiaryBlocker>()))
        PhysicsEngine.SkipCollision = true;
      if (this.IsCop && this.IsCopDead)
        PhysicsEngine.SkipCollision = true;
      if (GameManager.Instance.IsFoyer && (bool) (UnityEngine.Object) otherRigidbody.GetComponent<TalkDoerLite>())
        PhysicsEngine.SkipCollision = true;
      if (this.companionID != CompanionController.CompanionIdentifier.GATLING_GULL || !(bool) (UnityEngine.Object) otherRigidbody.aiActor || !(bool) (UnityEngine.Object) otherRigidbody.healthHaver || otherRigidbody.healthHaver.IsBoss)
        return;
      PhysicsEngine.SkipCollision = true;
    }

    private void CustomPitDeathHandling(AIActor actor, ref bool suppressDamage)
    {
      suppressDamage = true;
      if ((bool) (UnityEngine.Object) this.m_owner && this.m_owner.IsInMinecart)
      {
        this.StartCoroutine(this.DelayedPitReturn());
      }
      else
      {
        this.transform.position = this.m_owner.transform.position;
        this.specRigidbody.Reinitialize();
        this.aiActor.RecoverFromFall();
      }
    }

    [DebuggerHidden]
    private IEnumerator DelayedPitReturn()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new CompanionController__DelayedPitReturnc__Iterator4()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator ScoopPlayerToSafety()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new CompanionController__ScoopPlayerToSafetyc__Iterator5()
      {
        _this = this
      };
    }

    private void UpdatePlayerPosition(tk2dBaseSprite obj)
    {
      if (!(bool) (UnityEngine.Object) this.m_owner || !(bool) (UnityEngine.Object) obj)
        return;
      Transform transform = obj.transform.Find("carry");
      if (!(bool) (UnityEngine.Object) transform)
        return;
      this.m_owner.transform.position = (Vector3) (transform.position.XY() + (this.m_owner.transform.position.XY() - this.m_owner.sprite.WorldBottomCenter) + new Vector2(0.0f, -0.125f));
      this.m_owner.specRigidbody.Reinitialize();
    }

    public void DoPet(PlayerController player)
    {
      this.aiAnimator.LockFacingDirection = true;
      if ((double) this.specRigidbody.UnitCenter.x > (double) player.specRigidbody.UnitCenter.x)
      {
        this.aiAnimator.FacingDirection = 180f;
        this.m_petOffset = new Vector2(5f / 16f, -0.625f);
      }
      else
      {
        this.aiAnimator.FacingDirection = 0.0f;
        this.m_petOffset = new Vector2(-13f / 16f, -0.625f);
      }
      this.aiAnimator.PlayUntilCancelled("pet");
      this.m_pettingDoer = player;
    }

    public void StopPet()
    {
      if (!((UnityEngine.Object) this.m_pettingDoer != (UnityEngine.Object) null))
        return;
      this.aiAnimator.EndAnimationIf("pet");
      this.aiAnimator.LockFacingDirection = false;
      this.m_pettingDoer = (PlayerController) null;
    }

    public bool IsBeingPet => (UnityEngine.Object) this.m_pettingDoer != (UnityEngine.Object) null;

    protected override void OnDestroy()
    {
      if ((bool) (UnityEngine.Object) this.m_owner)
      {
        this.m_owner.OnUsedPlayerItem -= new Action<PlayerController, PlayerItem>(this.HandleItemUsed);
        this.m_owner.companions.Remove(this.aiActor);
        this.m_owner.OnPitfall -= new System.Action(this.HandlePitfall);
        this.m_owner.OnRoomClearEvent -= new Action<PlayerController>(this.HandleRoomCleared);
      }
      if ((bool) (UnityEngine.Object) this.aiShooter)
        this.aiShooter.PostProcessProjectile -= new Action<Projectile>(this.HandleCompanionPostProcessProjectile);
      base.OnDestroy();
    }

    public enum CompanionIdentifier
    {
      NONE,
      SUPER_SPACE_TURTLE,
      PAYDAY_SHOOT,
      PAYDAY_BLOCK,
      PAYDAY_STUN,
      BABY_GOOD_MIMIC,
      PHOENIX,
      PIG,
      SHELLETON,
      GATLING_GULL,
    }
  }

