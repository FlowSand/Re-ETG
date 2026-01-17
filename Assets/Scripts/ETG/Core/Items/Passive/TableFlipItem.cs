// Decompiled with JetBrains decompiler
// Type: TableFlipItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class TableFlipItem : PassiveItem
    {
      public bool TableTriggersBlankEffect;
      public bool TableStunsEnemies;
      [ShowInInspectorIf("TableStunsEnemies", false)]
      public float ChanceToStun = 1f;
      [ShowInInspectorIf("TableStunsEnemies", false)]
      public float StunDuration = 4f;
      [ShowInInspectorIf("TableStunsEnemies", false)]
      public float StunRadius = 10f;
      [ShowInInspectorIf("TableStunsEnemies", false)]
      public bool StunsAllEnemiesInRoom;
      public bool TableGivesCurrency;
      [ShowInInspectorIf("TableGivesCurrency", false)]
      public float ChanceToGiveCurrency = 1f;
      [ShowInInspectorIf("TableGivesCurrency", false)]
      public int CurrencyToGiveMin = 1;
      [ShowInInspectorIf("TableGivesCurrency", false)]
      public int CurrencyToGiveMax = 1;
      public bool TableGivesRage;
      [ShowInInspectorIf("TableGivesRage", false)]
      public float RageDamageMultiplier = 2f;
      [ShowInInspectorIf("TableGivesRage", false)]
      public float RageDuration = 5f;
      [ShowInInspectorIf("TableGivesRage", false)]
      public Color RageFlatColor = new Color(0.5f, 0.0f, 0.0f, 0.75f);
      [ShowInInspectorIf("TableGivesRage", false)]
      public GameObject RageOverheadVFX;
      public bool AddsModuleCopies;
      [ShowInInspectorIf("AddsModuleCopies", false)]
      public float ModuleCopyDuration = 5f;
      [ShowInInspectorIf("AddsModuleCopies", false)]
      public int ModuleCopyCount = 1;
      public bool TableBecomesProjectile;
      [ShowInInspectorIf("TableBecomesProjectile", false)]
      public ExplosionData ProjectileExplosionData;
      [ShowInInspectorIf("TableBecomesProjectile", false)]
      public float DirectHitBonusDamage = 10f;
      [ShowInInspectorIf("TableBecomesProjectile", false)]
      public AnimationCurve CustomAccelerationCurve;
      [ShowInInspectorIf("TableBecomesProjectile", false)]
      public float CustomAccelerationCurveDuration;
      public bool TableSlowsTime;
      [ShowInInspectorIf("TableSlowsTime", false)]
      public float SlowTimeAmount = 0.5f;
      [ShowInInspectorIf("TableSlowsTime", false)]
      public float SlowTimeDuration = 3f;
      public bool TableProvidesInvulnerability;
      [ShowInInspectorIf("TableProvidesInvulnerability", false)]
      public float InvulnerableTimeDuration = 3f;
      public bool TableFlocking;
      [Space(10f)]
      public bool TableFiresVolley;
      [ShowInInspectorIf("TableFiresVolley", false)]
      public ProjectileVolleyData Volley;
      [LongNumericEnum]
      public List<CustomSynergyType> VolleyOverrideSynergies;
      public List<ProjectileVolleyData> VolleyOverrides;
      [Space(10f)]
      public bool TableHeat;
      [ShowInInspectorIf("TableHeat", false)]
      public float TableHeatRadius = 5f;
      [ShowInInspectorIf("TableHeat", false)]
      public float TableHeatSynergyRadius = 20f;
      [ShowInInspectorIf("TableHeat", false)]
      public float TableHeatDuration = 5f;
      public GameActorFireEffect TableHeatEffect;
      public GameActorFireEffect TableHeatSynergyEffect;
      [Header("Other Synergies")]
      public bool UsesTableTechBeesSynergy;
      [ShowInInspectorIf("UsesTableTechBeesSynergy", false)]
      public Projectile BeeProjectile;
      public int MinNumberOfBeesPerEnemyBullet = 1;
      public int MaxNumberOfBeesPerEnemyBullet = 1;
      public bool UsesTimeSlowSynergy;
      [LongNumericEnum]
      public CustomSynergyType TimeSlowRequiredSynergy;
      [ShowInInspectorIf("UsesTimeSlowSynergy", false)]
      public RadialSlowInterface RadialSlow;
      private const int c_beeCap = 49;
      private int m_beeCount;
      private static bool TableFlipTimeIsActive;
      private static float AdditionalTableFlipSlowTime;
      private float m_rageElapsed;
      private GameObject rageInstanceVFX;
      private float m_volleyElapsed = -1f;
      private Dictionary<FlippableCover, HeatIndicatorController> m_radialIndicators;

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        this.m_owner = player;
        base.Pickup(player);
        player.OnTableFlipped += new Action<FlippableCover>(this.DoEffect);
        player.OnTableFlipCompleted += new Action<FlippableCover>(this.DoEffectCompleted);
      }

      private void DoEffect(FlippableCover obj)
      {
        this.HandleBlankEffect(obj);
        this.HandleStunEffect();
        this.HandleRageEffect();
        this.HandleVolleyEffect();
        this.StartCoroutine(this.HandleDelayedEffect(0.25f, new Action<FlippableCover>(this.HandleTableVolley), obj));
        this.HandleTemporalEffects();
        this.HandleHeatEffects(obj);
        if (!this.UsesTimeSlowSynergy || !(bool) (UnityEngine.Object) this.Owner || !this.Owner.HasActiveBonusSynergy(this.TimeSlowRequiredSynergy))
          return;
        this.RadialSlow.DoRadialSlow(this.Owner.CenterPosition, this.Owner.CurrentRoom);
      }

      private void DoEffectCompleted(FlippableCover obj)
      {
        this.HandleMoneyEffect(obj);
        this.HandleProjectileEffect(obj);
        this.HandleTableFlocking(obj);
      }

      [DebuggerHidden]
      private IEnumerator HandleDelayedEffect(
        float delayTime,
        Action<FlippableCover> effect,
        FlippableCover table)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TableFlipItem.<HandleDelayedEffect>c__Iterator0()
        {
          delayTime = delayTime,
          effect = effect,
          table = table
        };
      }

      private void HandleTableVolley(FlippableCover table)
      {
        if (!this.TableFiresVolley)
          return;
        IntVector2 vector2FromDirection = DungeonData.GetIntVector2FromDirection(table.DirectionFlipped);
        ProjectileVolleyData sourceVolley = this.Volley;
        float num = 1f;
        if (this.VolleyOverrideSynergies != null)
        {
          for (int index = 0; index < this.VolleyOverrideSynergies.Count; ++index)
          {
            if ((bool) (UnityEngine.Object) this.m_owner && this.m_owner.HasActiveBonusSynergy(this.VolleyOverrideSynergies[index]))
            {
              sourceVolley = this.VolleyOverrides[index];
              num = 2f;
            }
          }
        }
        VolleyUtility.FireVolley(sourceVolley, table.sprite.WorldCenter + vector2FromDirection.ToVector2() * num, vector2FromDirection.ToVector2(), (GameActor) this.m_owner);
      }

      private void HandleTableFlocking(FlippableCover table)
      {
        if (!this.TableFlocking)
          return;
        RoomHandler currentRoom = this.Owner.CurrentRoom;
        ReadOnlyCollection<IPlayerInteractable> roomInteractables = currentRoom.GetRoomInteractables();
        for (int index = 0; index < roomInteractables.Count; ++index)
        {
          if (currentRoom.IsRegistered(roomInteractables[index]))
          {
            FlippableCover flippableCover = roomInteractables[index] as FlippableCover;
            if ((UnityEngine.Object) flippableCover != (UnityEngine.Object) null && !flippableCover.IsFlipped && !flippableCover.IsGilded)
            {
              if (flippableCover.flipStyle == FlippableCover.FlipStyle.ANY)
              {
                flippableCover.ForceSetFlipper(this.Owner);
                flippableCover.Flip(table.DirectionFlipped);
              }
              else if (flippableCover.flipStyle == FlippableCover.FlipStyle.ONLY_FLIPS_LEFT_RIGHT)
              {
                if (table.DirectionFlipped == DungeonData.Direction.NORTH || table.DirectionFlipped == DungeonData.Direction.SOUTH)
                {
                  flippableCover.ForceSetFlipper(this.Owner);
                  flippableCover.Flip((double) UnityEngine.Random.value <= 0.5 ? DungeonData.Direction.WEST : DungeonData.Direction.EAST);
                }
                else
                {
                  flippableCover.ForceSetFlipper(this.Owner);
                  flippableCover.Flip(table.DirectionFlipped);
                }
              }
              else if (flippableCover.flipStyle == FlippableCover.FlipStyle.ONLY_FLIPS_UP_DOWN)
              {
                if (table.DirectionFlipped == DungeonData.Direction.EAST || table.DirectionFlipped == DungeonData.Direction.WEST)
                {
                  flippableCover.ForceSetFlipper(this.Owner);
                  flippableCover.Flip((double) UnityEngine.Random.value <= 0.5 ? DungeonData.Direction.SOUTH : DungeonData.Direction.NORTH);
                }
                else
                {
                  flippableCover.ForceSetFlipper(this.Owner);
                  flippableCover.Flip(table.DirectionFlipped);
                }
              }
            }
          }
        }
      }

      private void HandleProjectileEffect(FlippableCover table)
      {
        if (!this.TableBecomesProjectile)
          return;
        GameObject original = (GameObject) ResourceCache.Acquire("Global VFX/VFX_Table_Exhaust");
        Vector2 vector2 = DungeonData.GetIntVector2FromDirection(table.DirectionFlipped).ToVector2();
        float z = BraveMathCollege.Atan2Degrees(vector2);
        Vector3 vector3 = Vector3.zero;
        switch (table.DirectionFlipped)
        {
          case DungeonData.Direction.NORTH:
            vector3 = Vector3.zero;
            break;
          case DungeonData.Direction.EAST:
            vector3 = new Vector3(-0.5f, 0.25f, 0.0f);
            break;
          case DungeonData.Direction.SOUTH:
            vector3 = new Vector3(0.0f, 0.5f, 1f);
            break;
          case DungeonData.Direction.WEST:
            vector3 = new Vector3(0.5f, 0.25f, 0.0f);
            break;
        }
        UnityEngine.Object.Instantiate<GameObject>(original, table.specRigidbody.UnitCenter.ToVector3ZisY() + vector3, Quaternion.Euler(0.0f, 0.0f, z)).transform.parent = table.specRigidbody.transform;
        Projectile source = table.specRigidbody.gameObject.AddComponent<Projectile>();
        source.Shooter = this.Owner.specRigidbody;
        source.Owner = (GameActor) this.Owner;
        source.baseData.damage = this.DirectHitBonusDamage;
        source.baseData.range = 1000f;
        source.baseData.speed = 20f;
        source.baseData.force = 50f;
        source.baseData.UsesCustomAccelerationCurve = true;
        source.baseData.AccelerationCurve = this.CustomAccelerationCurve;
        source.baseData.CustomAccelerationCurveDuration = this.CustomAccelerationCurveDuration;
        source.shouldRotate = false;
        source.Start();
        source.SendInDirection(vector2, true);
        source.collidesWithProjectiles = true;
        source.projectileHitHealth = 20;
        Action<Projectile> action = (Action<Projectile>) (p =>
        {
          if (!(bool) (UnityEngine.Object) table || !(bool) (UnityEngine.Object) table.shadowSprite)
            return;
          table.shadowSprite.renderer.enabled = false;
        });
        source.OnDestruction += action;
        source.gameObject.AddComponent<ExplosiveModifier>().explosionData = this.ProjectileExplosionData;
        table.PreventPitFalls = true;
        if (!(bool) (UnityEngine.Object) this.Owner || !this.Owner.HasActiveBonusSynergy(CustomSynergyType.ROCKET_POWERED_TABLES))
          return;
        HomingModifier homingModifier = source.gameObject.AddComponent<HomingModifier>();
        homingModifier.AssignProjectile(source);
        homingModifier.HomingRadius = 20f;
        homingModifier.AngularVelocity = 720f;
        BounceProjModifier bounceProjModifier = source.gameObject.AddComponent<BounceProjModifier>();
        bounceProjModifier.numberOfBounces = 4;
        bounceProjModifier.onlyBounceOffTiles = true;
      }

      private void HandleBlankEffect(FlippableCover table)
      {
        if (!this.TableTriggersBlankEffect)
          return;
        GameManager.Instance.StartCoroutine(this.DelayedBlankEffect(table));
      }

      [DebuggerHidden]
      private IEnumerator DelayedBlankEffect(FlippableCover table)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TableFlipItem.<DelayedBlankEffect>c__Iterator1()
        {
          table = table,
          _this = this
        };
      }

      private void PostProcessTableTechBees(Projectile target)
      {
        for (int index = 0; index < UnityEngine.Random.Range(this.MinNumberOfBeesPerEnemyBullet, this.MaxNumberOfBeesPerEnemyBullet); ++index)
        {
          if ((bool) (UnityEngine.Object) target && (bool) (UnityEngine.Object) this.Owner && this.m_beeCount < 49)
          {
            ++this.m_beeCount;
            Projectile component = SpawnManager.SpawnProjectile(this.BeeProjectile.gameObject, target.transform.position + UnityEngine.Random.insideUnitCircle.ToVector3ZisY(), target.transform.rotation).GetComponent<Projectile>();
            component.Owner = (GameActor) this.Owner;
            component.Shooter = this.Owner.specRigidbody;
            component.collidesWithPlayer = false;
            component.collidesWithEnemies = true;
            component.collidesWithProjectiles = false;
          }
        }
      }

      private void InternalForceBlank(
        Vector2 center,
        float overrideRadius = 25f,
        float overrideTimeAtMaxRadius = 0.5f,
        bool silent = false,
        bool breaksWalls = true,
        bool breaksObjects = true,
        float overrideForce = -1f,
        Action<Projectile> customCallback = null)
      {
        GameObject silencerVFX = !silent ? (GameObject) BraveResources.Load("Global VFX/BlankVFX") : (GameObject) null;
        if (!silent)
        {
          int num1 = (int) AkSoundEngine.PostEvent("Play_OBJ_silenceblank_use_01", this.gameObject);
          int num2 = (int) AkSoundEngine.PostEvent("Stop_ENM_attack_cancel_01", this.gameObject);
        }
        SilencerInstance silencerInstance = new GameObject("silencer").AddComponent<SilencerInstance>();
        float additionalTimeAtMaxRadius = overrideTimeAtMaxRadius;
        if (customCallback != null)
        {
          silencerInstance.UsesCustomProjectileCallback = true;
          silencerInstance.OnCustomBlankedProjectile = customCallback;
        }
        silencerInstance.TriggerSilencer(center, 50f, overrideRadius, silencerVFX, !silent ? 0.15f : 0.0f, !silent ? 0.2f : 0.0f, !silent ? 50f : 0.0f, !silent ? 10f : 0.0f, !silent ? ((double) overrideForce < 0.0 ? 140f : overrideForce) : 0.0f, !breaksObjects ? 0.0f : (!silent ? 15f : 5f), additionalTimeAtMaxRadius, this.Owner, breaksWalls);
        if (!(bool) (UnityEngine.Object) this.Owner)
          return;
        this.Owner.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
      }

      private void HandleStunEffect()
      {
        if (!this.TableStunsEnemies)
          return;
        List<AIActor> activeEnemies = this.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
        if (activeEnemies == null)
          return;
        for (int index = 0; index < activeEnemies.Count; ++index)
        {
          if ((double) UnityEngine.Random.value < (double) this.ChanceToStun)
          {
            if (this.StunsAllEnemiesInRoom)
              this.StunEnemy(activeEnemies[index]);
            else if ((double) Vector2.Distance(activeEnemies[index].CenterPosition, this.Owner.CenterPosition) < (double) this.StunRadius)
              this.StunEnemy(activeEnemies[index]);
          }
        }
      }

      private void StunEnemy(AIActor enemy)
      {
        if (enemy.healthHaver.IsBoss || !(bool) (UnityEngine.Object) enemy || !(bool) (UnityEngine.Object) enemy.behaviorSpeculator)
          return;
        enemy.ClearPath();
        enemy.behaviorSpeculator.Interrupt();
        enemy.behaviorSpeculator.Stun(this.StunDuration);
      }

      private void HandleMoneyEffect(FlippableCover sourceCover)
      {
        if (!this.TableGivesCurrency || (double) UnityEngine.Random.value >= (double) this.ChanceToGiveCurrency)
          return;
        int amountToDrop = UnityEngine.Random.Range(this.CurrencyToGiveMin, this.CurrencyToGiveMax);
        LootEngine.SpawnCurrency(sourceCover.specRigidbody.UnitCenter, amountToDrop);
      }

      private void HandleTemporalEffects()
      {
        if (this.TableSlowsTime && (!this.UsesTimeSlowSynergy || !(bool) (UnityEngine.Object) this.Owner || !this.Owner.HasActiveBonusSynergy(this.TimeSlowRequiredSynergy)))
          this.Owner.StartCoroutine(this.HandleTimeSlowDuration());
        if (!this.TableProvidesInvulnerability)
          return;
        this.Owner.healthHaver.TriggerInvulnerabilityPeriod(this.InvulnerableTimeDuration);
      }

      [DebuggerHidden]
      private IEnumerator HandleTimeSlowDuration()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TableFlipItem.<HandleTimeSlowDuration>c__Iterator2()
        {
          _this = this
        };
      }

      private void HandleRageEffect()
      {
        if (!this.TableGivesRage)
          return;
        if ((double) this.m_rageElapsed > 0.0)
        {
          this.m_rageElapsed = this.RageDuration;
          if (this.Owner.HasActiveBonusSynergy(CustomSynergyType.ANGRIER_BULLETS))
            this.m_rageElapsed *= 3f;
          if (!(bool) (UnityEngine.Object) this.RageOverheadVFX || !((UnityEngine.Object) this.rageInstanceVFX == (UnityEngine.Object) null))
            return;
          this.rageInstanceVFX = this.Owner.PlayEffectOnActor(this.RageOverheadVFX, new Vector3(0.0f, 1.375f, 0.0f), alreadyMiddleCenter: true);
        }
        else
          this.Owner.StartCoroutine(this.HandleRageCooldown());
      }

      [DebuggerHidden]
      private IEnumerator HandleRageCooldown()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TableFlipItem.<HandleRageCooldown>c__Iterator3()
        {
          _this = this
        };
      }

      private void HandleVolleyEffect()
      {
        if (!this.AddsModuleCopies)
          return;
        if ((double) this.m_volleyElapsed < 0.0)
          this.Owner.StartCoroutine(this.HandleVolleyCooldown());
        else
          this.m_volleyElapsed = 0.0f;
      }

      [DebuggerHidden]
      private IEnumerator HandleVolleyCooldown()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TableFlipItem.<HandleVolleyCooldown>c__Iterator4()
        {
          _this = this
        };
      }

      private void HandleHeatEffects(FlippableCover table)
      {
        if (!this.TableHeat || !(bool) (UnityEngine.Object) table)
          return;
        table.StartCoroutine(this.HandleHeatEffectsCR(table));
      }

      [DebuggerHidden]
      private IEnumerator HandleHeatEffectsCR(FlippableCover table)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new TableFlipItem.<HandleHeatEffectsCR>c__Iterator5()
        {
          table = table,
          _this = this
        };
      }

      private void HandleRadialIndicator(FlippableCover table)
      {
        if (this.m_radialIndicators == null)
          this.m_radialIndicators = new Dictionary<FlippableCover, HeatIndicatorController>();
        if (this.m_radialIndicators.ContainsKey(table))
          return;
        Vector3 position = !(bool) (UnityEngine.Object) table.sprite ? table.transform.position : table.sprite.WorldCenter.ToVector3ZisY();
        this.m_radialIndicators.Add(table, ((GameObject) UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), position, Quaternion.identity, table.transform)).GetComponent<HeatIndicatorController>());
        int count = -1;
        float num = !PlayerController.AnyoneHasActiveBonusSynergy(CustomSynergyType.HIDDEN_TECH_FLARE, out count) ? this.TableHeatRadius : this.TableHeatSynergyRadius;
        this.m_radialIndicators[table].CurrentRadius = num;
      }

      private void UnhandleRadialIndicator(FlippableCover table)
      {
        if (!this.m_radialIndicators.ContainsKey(table))
          return;
        this.m_radialIndicators[table].EndEffect();
        this.m_radialIndicators.Remove(table);
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        debrisObject.GetComponent<TableFlipItem>().m_pickedUpThisRun = true;
        if ((bool) (UnityEngine.Object) player)
        {
          player.OnTableFlipped -= new Action<FlippableCover>(this.DoEffect);
          player.OnTableFlipCompleted -= new Action<FlippableCover>(this.DoEffectCompleted);
        }
        this.m_owner = (PlayerController) null;
        return debrisObject;
      }

      protected override void OnDestroy()
      {
        this.m_radialIndicators = (Dictionary<FlippableCover, HeatIndicatorController>) null;
        BraveTime.ClearMultiplier(this.gameObject);
        if ((bool) (UnityEngine.Object) this.Owner)
        {
          this.Owner.OnTableFlipped -= new Action<FlippableCover>(this.DoEffect);
          this.Owner.OnTableFlipCompleted -= new Action<FlippableCover>(this.DoEffectCompleted);
        }
        base.OnDestroy();
      }

      public void ModifyVolley(ProjectileVolleyData volleyToModify)
      {
        if (this.ModuleCopyCount <= 0)
          return;
        int count = volleyToModify.projectiles.Count;
        for (int index1 = 0; index1 < count; ++index1)
        {
          ProjectileModule projectile = volleyToModify.projectiles[index1];
          float num1 = (float) ((double) this.ModuleCopyCount * 10.0 * -1.0 / 2.0);
          for (int index2 = 0; index2 < this.ModuleCopyCount; ++index2)
          {
            int sourceIndex = index1;
            if (projectile.CloneSourceIndex >= 0)
              sourceIndex = projectile.CloneSourceIndex;
            ProjectileModule clone = ProjectileModule.CreateClone(projectile, false, sourceIndex);
            float num2 = num1 + 10f * (float) index2;
            clone.angleFromAim = num2;
            clone.ignoredForReloadPurposes = true;
            clone.ammoCost = 0;
            volleyToModify.projectiles.Add(clone);
          }
        }
      }
    }

}
