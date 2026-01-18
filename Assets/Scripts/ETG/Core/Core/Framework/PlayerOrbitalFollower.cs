// Decompiled with JetBrains decompiler
// Type: PlayerOrbitalFollower
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class PlayerOrbitalFollower : BraveBehaviour
  {
    public Projectile shootProjectile;
    public float shootCooldown = 1f;
    public bool shouldRotate;
    public float rotationOffset;
    public float maxRotationDegreesPerSecond = 360f;
    public bool BlanksOnProjectileRadius;
    public float BlankRadius = 4f;
    public float BlankFrequency = 3f;
    [CheckAnimation(null)]
    public string BlankAnimationName;
    [CheckAnimation(null)]
    public string BlankIdleName;
    public string IdleAnimation;
    [Header("Synergies")]
    public PlayerOrbitalSynergyData[] synergies;
    [NonSerialized]
    public bool OverridePosition;
    [NonSerialized]
    public Vector3 OverrideTargetPosition = Vector3.zero;
    public bool PredictsChests;
    private bool m_initialized;
    private PlayerController m_owner;
    private AIActor m_currentTarget;
    private float m_shootTimer;
    private float m_retargetTimer;
    private int m_orbitalIndex;
    private float m_targetAngle;
    private float m_blankCooldown;
    private bool m_hasLuteBuff;
    private GameObject m_luteOverheadVfx;
    private GameObject BlankVFXPrefab;
    private Chest m_lastPredictedChest;
    private Vector2 m_lastTargetMotionVector;
    private Vector2 m_lastOwnerCenter;
    private const float DIST_BETWEEN_AT_REST = 1.25f;

    public void Initialize(PlayerController owner)
    {
      this.m_initialized = true;
      this.m_owner = owner;
      this.m_orbitalIndex = owner.trailOrbitals.Count;
      owner.trailOrbitals.Add(this);
      this.sprite = (tk2dBaseSprite) this.GetComponentInChildren<tk2dSprite>();
      this.spriteAnimator = this.GetComponentInChildren<tk2dSpriteAnimator>();
      SpriteOutlineManager.AddOutlineToSprite(this.sprite, Color.black);
    }

    private void DoMicroBlank()
    {
      if ((UnityEngine.Object) this.BlankVFXPrefab == (UnityEngine.Object) null)
        this.BlankVFXPrefab = (GameObject) BraveResources.Load("Global VFX/BlankVFX_Ghost");
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", this.gameObject);
      SilencerInstance silencerInstance = new GameObject("silencer").AddComponent<SilencerInstance>();
      float additionalTimeAtMaxRadius = 0.25f;
      if ((bool) (UnityEngine.Object) this.sprite && (bool) (UnityEngine.Object) this.sprite.spriteAnimator && this.sprite.spriteAnimator.GetClipByName(this.BlankAnimationName) != null)
        this.sprite.spriteAnimator.PlayForDuration(this.BlankAnimationName, -1f, this.BlankIdleName);
      silencerInstance.TriggerSilencer(this.specRigidbody.UnitCenter, 20f, this.BlankRadius, this.BlankVFXPrefab, 0.0f, 3f, 3f, 3f, 30f, 3f, additionalTimeAtMaxRadius, this.m_owner, false);
    }

    public void ToggleRenderer(bool value)
    {
      this.sprite.renderer.enabled = value;
      SpriteOutlineManager.ToggleOutlineRenderers(this.sprite, value);
    }

    private void Update()
    {
      if (!this.m_initialized)
        return;
      this.HandleMotion();
      this.HandleCombat();
      bool flag1 = false;
      bool flag2 = false;
      if (this.synergies != null && (bool) (UnityEngine.Object) this.m_owner && (bool) (UnityEngine.Object) this.spriteAnimator)
      {
        for (int index = 0; index < this.synergies.Length; ++index)
        {
          PlayerOrbitalSynergyData synergy = this.synergies[index];
          flag1 |= synergy.HasOverrideAnimations;
          if (synergy.HasOverrideAnimations && this.m_owner.HasActiveBonusSynergy(synergy.SynergyToCheck))
          {
            flag2 = true;
            if (!this.spriteAnimator.IsPlaying(synergy.OverrideIdleAnimation))
              this.spriteAnimator.Play(synergy.OverrideIdleAnimation);
          }
        }
      }
      if (flag1 && !flag2 && !string.IsNullOrEmpty(this.IdleAnimation) && (bool) (UnityEngine.Object) this.spriteAnimator && !this.spriteAnimator.IsPlaying(this.IdleAnimation))
        this.spriteAnimator.Play(this.IdleAnimation);
      if (this.BlanksOnProjectileRadius)
      {
        this.m_blankCooldown -= BraveTime.DeltaTime;
        if ((double) this.m_blankCooldown <= 0.0)
        {
          this.HandleBlanks();
          this.m_blankCooldown = this.BlankFrequency;
        }
      }
      if (this.shouldRotate)
      {
        float num = Mathf.MoveTowardsAngle(this.transform.rotation.eulerAngles.z, this.m_targetAngle + this.rotationOffset, this.maxRotationDegreesPerSecond * BraveTime.DeltaTime);
        if (float.IsNaN(num) || float.IsInfinity(num))
          num = 0.0f;
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, num);
      }
      this.m_retargetTimer -= BraveTime.DeltaTime;
      this.m_shootTimer -= BraveTime.DeltaTime;
      if (this.PredictsChests)
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
            this.GetComponent<HologramDoer>().HideSprite(this.gameObject);
          if ((bool) (UnityEngine.Object) chest)
          {
            List<PickupObject> pickupObjectList = chest.PredictContents(this.m_owner);
            if (pickupObjectList.Count > 0 && (bool) (UnityEngine.Object) pickupObjectList[0].encounterTrackable)
            {
              tk2dSpriteCollectionData encounterIconCollection = AmmonomiconController.ForceInstance.EncounterIconCollection;
              this.GetComponent<HologramDoer>().ChangeToSprite(this.gameObject, encounterIconCollection, encounterIconCollection.GetSpriteIdByName(pickupObjectList[0].encounterTrackable.journalData.AmmonomiconSprite));
            }
          }
          this.m_lastPredictedChest = chest;
        }
      }
      if (!(bool) (UnityEngine.Object) this.shootProjectile || !(bool) (UnityEngine.Object) this.specRigidbody)
        return;
      if (this.m_hasLuteBuff && (!(bool) (UnityEngine.Object) this.m_owner || !(bool) (UnityEngine.Object) this.m_owner.CurrentGun || !this.m_owner.CurrentGun.LuteCompanionBuffActive))
      {
        if ((bool) (UnityEngine.Object) this.m_luteOverheadVfx)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_luteOverheadVfx);
          this.m_luteOverheadVfx = (GameObject) null;
        }
        if ((bool) (UnityEngine.Object) this.specRigidbody)
          this.specRigidbody.OnPostRigidbodyMovement -= new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.UpdateVFXOnMovement);
        this.m_hasLuteBuff = false;
      }
      else
      {
        if (this.m_hasLuteBuff || !(bool) (UnityEngine.Object) this.m_owner || !(bool) (UnityEngine.Object) this.m_owner.CurrentGun || !this.m_owner.CurrentGun.LuteCompanionBuffActive)
          return;
        this.m_luteOverheadVfx = SpawnManager.SpawnVFX((GameObject) ResourceCache.Acquire("Global VFX/VFX_Buff_Status"), this.specRigidbody.UnitCenter.ToVector3ZisY().Quantize(1f / 16f) + new Vector3(0.0f, 1f, 0.0f), Quaternion.identity);
        if ((bool) (UnityEngine.Object) this.specRigidbody)
          this.specRigidbody.OnPostRigidbodyMovement += new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.UpdateVFXOnMovement);
        this.m_hasLuteBuff = true;
      }
    }

    private void UpdateVFXOnMovement(SpeculativeRigidbody arg1, Vector2 arg2, IntVector2 arg3)
    {
      if (!this.m_hasLuteBuff || !(bool) (UnityEngine.Object) this.m_luteOverheadVfx)
        return;
      this.m_luteOverheadVfx.transform.position = this.specRigidbody.UnitCenter.ToVector3ZisY().Quantize(1f / 16f) + new Vector3(0.0f, 1f, 0.0f);
    }

    protected override void OnDestroy()
    {
      for (int index = 0; index < this.m_owner.trailOrbitals.Count; ++index)
      {
        if (this.m_owner.trailOrbitals[index].m_orbitalIndex > this.m_orbitalIndex)
          --this.m_owner.trailOrbitals[index].m_orbitalIndex;
      }
      this.m_owner.trailOrbitals.Remove(this);
    }

    private void HandleBlanks()
    {
      Vector2 unitCenter = this.specRigidbody.UnitCenter;
      float num = this.BlankRadius * this.BlankRadius;
      for (int index = 0; index < StaticReferenceManager.AllProjectiles.Count; ++index)
      {
        Projectile allProjectile = StaticReferenceManager.AllProjectiles[index];
        if ((bool) (UnityEngine.Object) allProjectile && allProjectile.collidesWithPlayer && (bool) (UnityEngine.Object) allProjectile.specRigidbody && (!(bool) (UnityEngine.Object) this.m_owner || !(allProjectile.Owner is PlayerController)) && (double) (unitCenter - allProjectile.specRigidbody.UnitCenter).sqrMagnitude < (double) num)
        {
          this.DoMicroBlank();
          break;
        }
      }
    }

    private void HandleMotion()
    {
      Vector2 vector2_1 = this.m_owner.CenterPosition;
      if (this.m_orbitalIndex > 0)
        vector2_1 = this.m_owner.trailOrbitals[this.m_orbitalIndex - 1].specRigidbody.UnitCenter;
      Vector2 vector2_2 = vector2_1 - this.m_lastOwnerCenter;
      if ((double) vector2_2.sqrMagnitude > 0.0)
        this.m_lastTargetMotionVector = vector2_2.normalized;
      Vector2 vector2_3 = (vector2_1 + -1f * this.m_owner.trailOrbitals[0].m_lastTargetMotionVector * 1.25f).Quantize(1f / 16f);
      if (this.OverridePosition)
        vector2_3 = this.OverrideTargetPosition.XY();
      float magnitude = (vector2_3 - this.transform.position.XY()).magnitude;
      float a = Mathf.Lerp(0.1f, 15f, magnitude / 4f) * BraveTime.DeltaTime;
      if (this.OverridePosition)
        a = 15f * BraveTime.DeltaTime;
      float num = Mathf.Min(a, magnitude);
      this.specRigidbody.Velocity = (vector2_3 - this.transform.position.XY()).normalized * num / BraveTime.DeltaTime;
      if (this.shouldRotate)
        this.m_targetAngle = BraveMathCollege.Atan2Degrees((vector2_1 - this.transform.position.XY()).normalized);
      this.m_lastOwnerCenter = vector2_1;
    }

    private void AcquireTarget()
    {
      this.m_retargetTimer = 0.25f;
      this.m_currentTarget = (AIActor) null;
      if ((UnityEngine.Object) this.m_owner == (UnityEngine.Object) null || this.m_owner.CurrentRoom == null)
        return;
      List<AIActor> activeEnemies = this.m_owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
      if (activeEnemies == null || activeEnemies.Count <= 0)
        return;
      AIActor aiActor1 = (AIActor) null;
      float num1 = -1f;
      for (int index = 0; index < activeEnemies.Count; ++index)
      {
        AIActor aiActor2 = activeEnemies[index];
        if ((bool) (UnityEngine.Object) aiActor2 && aiActor2.HasBeenEngaged && aiActor2.IsWorthShootingAt)
        {
          float num2 = Vector2.Distance(this.transform.position.XY(), aiActor2.specRigidbody.UnitCenter);
          if ((UnityEngine.Object) aiActor1 == (UnityEngine.Object) null || (double) num2 < (double) num1)
          {
            aiActor1 = aiActor2;
            num1 = num2;
          }
        }
      }
      if (!(bool) (UnityEngine.Object) aiActor1)
        return;
      this.m_currentTarget = aiActor1;
    }

    private Vector2 FindPredictedTargetPosition()
    {
      float num1 = this.shootProjectile.baseData.speed;
      if ((double) num1 < 0.0)
        num1 = float.MaxValue;
      Vector2 a = this.transform.position.XY();
      Vector2 unitCenter = this.m_currentTarget.specRigidbody.HitboxPixelCollider.UnitCenter;
      float num2 = Vector2.Distance(a, unitCenter) / num1;
      return unitCenter + this.m_currentTarget.specRigidbody.Velocity * num2;
    }

    private void Shoot(Vector2 targetPosition)
    {
      Vector2 vector2 = targetPosition - this.transform.position.XY();
      float z = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
      Projectile component = SpawnManager.SpawnProjectile(this.shootProjectile.gameObject, (Vector3) this.transform.position.XY(), Quaternion.Euler(0.0f, 0.0f, z)).GetComponent<Projectile>();
      component.collidesWithEnemies = true;
      component.collidesWithPlayer = false;
      component.Owner = (GameActor) this.m_owner;
      component.Shooter = this.m_owner.specRigidbody;
      component.TreatedAsNonProjectileForChallenge = true;
      if (!(bool) (UnityEngine.Object) this.m_owner)
        return;
      if (PassiveItem.IsFlagSetForCharacter(this.m_owner, typeof (BattleStandardItem)))
        component.baseData.damage *= BattleStandardItem.BattleStandardCompanionDamageMultiplier;
      if ((bool) (UnityEngine.Object) this.m_owner.CurrentGun && this.m_owner.CurrentGun.LuteCompanionBuffActive)
      {
        component.baseData.damage *= 2f;
        component.RuntimeUpdateScale(1.75f);
      }
      this.m_owner.DoPostProcessProjectile(component);
    }

    private float GetModifiedCooldown()
    {
      float shootCooldown = this.shootCooldown;
      if (this.synergies != null && (bool) (UnityEngine.Object) this.m_owner)
      {
        for (int index = 0; index < this.synergies.Length; ++index)
        {
          if (this.m_owner.HasActiveBonusSynergy(this.synergies[index].SynergyToCheck))
            shootCooldown *= this.synergies[index].ShootCooldownMultiplier;
        }
      }
      if ((bool) (UnityEngine.Object) this.m_owner && (bool) (UnityEngine.Object) this.m_owner.CurrentGun && this.m_owner.CurrentGun.LuteCompanionBuffActive)
        shootCooldown /= 1.5f;
      return shootCooldown;
    }

    private void HandleCombat()
    {
      if (GameManager.Instance.IsPaused || !(bool) (UnityEngine.Object) this.m_owner || this.m_owner.CurrentInputState != PlayerInputState.AllInput || this.m_owner.IsInputOverridden)
        return;
      bool flag = false;
      for (int index = 0; index < this.synergies.Length; ++index)
      {
        if ((bool) (UnityEngine.Object) this.m_owner && this.m_owner.HasActiveBonusSynergy(this.synergies[index].SynergyToCheck) && this.synergies[index].EngagesFiring && (bool) (UnityEngine.Object) this.synergies[index].OverrideProjectile)
        {
          flag = true;
          break;
        }
      }
      if ((UnityEngine.Object) this.shootProjectile == (UnityEngine.Object) null || flag)
        return;
      if ((double) this.m_retargetTimer <= 0.0)
        this.m_currentTarget = (AIActor) null;
      if ((UnityEngine.Object) this.m_currentTarget == (UnityEngine.Object) null || !(bool) (UnityEngine.Object) this.m_currentTarget || this.m_currentTarget.healthHaver.IsDead)
        this.AcquireTarget();
      if ((UnityEngine.Object) this.m_currentTarget == (UnityEngine.Object) null)
        return;
      if (this.shouldRotate)
        this.m_targetAngle = BraveMathCollege.Atan2Degrees(this.m_currentTarget.CenterPosition - this.transform.position.XY());
      if ((double) this.m_shootTimer > 0.0)
        return;
      this.m_shootTimer = this.GetModifiedCooldown();
      Vector2 predictedTargetPosition = this.FindPredictedTargetPosition();
      if (this.m_owner.IsStealthed)
        return;
      this.Shoot(predictedTargetPosition);
    }
  }

