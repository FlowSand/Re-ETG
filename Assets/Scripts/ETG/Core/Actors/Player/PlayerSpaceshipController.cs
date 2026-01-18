// Decompiled with JetBrains decompiler
// Type: PlayerSpaceshipController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class PlayerSpaceshipController : PlayerController
  {
    public Texture2D PaletteTex;
    public List<Transform> LaserShootPoints;
    public tk2dSpriteAnimation TimefallCorpseLibrary;
    [Header("Spaceship Controls")]
    public float LaserACooldown = 0.15f;
    public float MissileCooldown = 1f;
    private float m_aimAngle;
    private float m_fireCooldown;
    private float m_missileCooldown;
    private bool m_isFiring;

    public override bool IsFlying => true;

    protected override bool CanDodgeRollWhileFlying => true;

    public override void Start()
    {
      base.Start();
      if ((Object) this.PaletteTex != (Object) null)
      {
        this.sprite.usesOverrideMaterial = true;
        this.sprite.renderer.material.SetTexture("_PaletteTex", (Texture) this.PaletteTex);
      }
      this.ToggleHandRenderers(false, "ships don't have hands");
      this.sprite.IsPerpendicular = false;
      this.sprite.HeightOffGround = 3f;
      this.sprite.UpdateZDepth();
    }

    public override void Update()
    {
      base.Update();
      if (!this.AcceptingNonMotionInput)
      {
        this.m_isFiring = false;
        this.m_shouldContinueFiring = false;
      }
      if (!this.IsDodgeRolling)
        this.sprite.transform.parent.rotation = Quaternion.Euler(0.0f, 0.0f, (this.m_aimAngle - (float) BraveMathCollege.AngleToOctant(this.m_aimAngle) * -45f).Quantize(10f) - 90f);
      if (this.m_isFiring && (double) this.m_fireCooldown <= 0.0)
      {
        this.FireProjectiles();
        this.m_fireCooldown = this.LaserACooldown;
      }
      this.m_missileCooldown -= BraveTime.DeltaTime;
      this.m_fireCooldown -= BraveTime.DeltaTime;
    }

    protected void FireMissileVolley()
    {
      if ((double) this.m_missileCooldown > 0.0)
        return;
      for (int index1 = 0; index1 < this.LaserShootPoints.Count; ++index1)
      {
        for (int index2 = 0; index2 < 5; ++index2)
          this.FireBullet(this.LaserShootPoints[index1], (Vector2) (Quaternion.Euler(0.0f, 0.0f, this.m_aimAngle - 90f + Mathf.Lerp(-20f, 20f, (float) index2 / 4f)) * (Vector3) Vector2.up), "missile");
      }
      this.m_missileCooldown = this.MissileCooldown;
      if (!(bool) (Object) this.CurrentItem)
        return;
      float destroyTime = -1f;
      this.CurrentItem.timeCooldown = this.MissileCooldown;
      this.CurrentItem.Use((PlayerController) this, out destroyTime);
    }

    protected override void CheckSpawnEmergencyCrate()
    {
    }

    protected void FireProjectiles()
    {
      for (int index = 0; index < this.LaserShootPoints.Count; ++index)
        this.FireBullet(this.LaserShootPoints[index], (Vector2) (Quaternion.Euler(0.0f, 0.0f, this.m_aimAngle - 90f) * (Vector3) Vector2.up), "default");
    }

    private void FireBullet(Transform shootPoint, Vector2 dirVec, string bulletType)
    {
      Projectile component = this.bulletBank.CreateProjectileFromBank((Vector2) shootPoint.position, BraveMathCollege.Atan2Degrees(dirVec.normalized), bulletType).GetComponent<Projectile>();
      component.Owner = (GameActor) this;
      component.Shooter = this.specRigidbody;
      component.collidesWithPlayer = false;
      component.specRigidbody.RegisterSpecificCollisionException(this.specRigidbody);
    }

    protected override void Die(Vector2 finalDamageDirection)
    {
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.SINGLE_PLAYER || GameManager.Instance.NumberOfLivingPlayers == 0)
      {
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST)
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.DIE_IN_PAST);
        this.CurrentInputState = PlayerInputState.NoInput;
        if ((bool) (Object) this.CurrentGun)
          this.CurrentGun.CeaseAttack();
        GameManager.Instance.MainCameraController.OverridePosition = GameManager.Instance.MainCameraController.transform.position;
        GameManager.Instance.StartCoroutine(this.HandleDelayedEndGame());
      }
      this.gameObject.SetActive(false);
    }

    [DebuggerHidden]
    private IEnumerator HandleDelayedEndGame()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new PlayerSpaceshipController__HandleDelayedEndGamec__Iterator0()
      {
        _this = this
      };
    }

    public override void ResurrectFromBossKill()
    {
      if (!this.gameObject.activeSelf)
        this.gameObject.SetActive(true);
      Chest.ToggleCoopChests(false);
      this.healthHaver.FullHeal();
    }

    protected override string GetBaseAnimationName(
      Vector2 v,
      float unusedGunAngle,
      bool invertThresholds = false,
      bool forceTwoHands = false)
    {
      string baseAnimationName = string.Empty;
      switch (BraveMathCollege.AngleToOctant(this.m_aimAngle))
      {
        case 0:
          baseAnimationName = !this.m_isFiring ? "idle_n" : "fire_n";
          break;
        case 1:
          baseAnimationName = !this.m_isFiring ? "idle_ne" : "fire_ne";
          break;
        case 2:
          baseAnimationName = !this.m_isFiring ? "idle_e" : "fire_e";
          break;
        case 3:
          baseAnimationName = !this.m_isFiring ? "idle_se" : "fire_se";
          break;
        case 4:
          baseAnimationName = !this.m_isFiring ? "idle_s" : "fire_s";
          break;
        case 5:
          baseAnimationName = !this.m_isFiring ? "idle_sw" : "fire_sw";
          break;
        case 6:
          baseAnimationName = !this.m_isFiring ? "idle_w" : "fire_w";
          break;
        case 7:
          baseAnimationName = !this.m_isFiring ? "idle_nw" : "fire_nw";
          break;
      }
      return baseAnimationName;
    }

    protected override void PlayDodgeRollAnimation(Vector2 direction)
    {
      tk2dSpriteAnimationClip clip = (tk2dSpriteAnimationClip) null;
      direction.Normalize();
      if (this.m_dodgeRollState != PlayerController.DodgeRollState.PreRollDelay)
      {
        float angle = direction.ToAngle();
        int octant = BraveMathCollege.AngleToOctant(this.m_aimAngle);
        string name = (double) BraveMathCollege.ClampAngle180(angle - this.m_aimAngle) < 0.0 ? "dodgeroll_right_" : "dodgeroll_left_";
        switch (octant)
        {
          case 0:
            name += "n";
            break;
          case 1:
            name += "ne";
            break;
          case 2:
            name += "e";
            break;
          case 3:
            name += "se";
            break;
          case 4:
            name += "s";
            break;
          case 5:
            name += "sw";
            break;
          case 6:
            name += "w";
            break;
          case 7:
            name += "nw";
            break;
        }
        clip = this.spriteAnimator.GetClipByName(name);
      }
      if (clip == null)
        return;
      float overrideFps = (float) clip.frames.Length / this.rollStats.GetModifiedTime((PlayerController) this);
      this.spriteAnimator.Play(clip, 0.0f, overrideFps);
      this.m_handlingQueuedAnimation = true;
    }

    protected override void HandleFlipping(float gunAngle)
    {
    }

    protected override Vector2 HandlePlayerInput()
    {
      Vector2 direction = Vector2.zero;
      if (this.CurrentInputState != PlayerInputState.NoMovement)
        direction = this.AdjustInputVector(this.m_activeActions.Move.Vector, BraveInput.MagnetAngles.movementCardinal, BraveInput.MagnetAngles.movementOrdinal);
      if ((double) direction.magnitude > 1.0)
        direction.Normalize();
      this.HandleStartDodgeRoll(direction);
      CollisionData result = (CollisionData) null;
      if ((double) direction.x > 0.0099999997764825821 && PhysicsEngine.Instance.RigidbodyCast(this.specRigidbody, IntVector2.Right, out result, collideWithRigidbodies: false))
        direction.x = 0.0f;
      CollisionData.Pool.Free(ref result);
      if ((double) direction.x < -0.0099999997764825821 && PhysicsEngine.Instance.RigidbodyCast(this.specRigidbody, IntVector2.Left, out result, collideWithRigidbodies: false))
        direction.x = 0.0f;
      CollisionData.Pool.Free(ref result);
      if ((double) direction.y > 0.0099999997764825821 && PhysicsEngine.Instance.RigidbodyCast(this.specRigidbody, IntVector2.Up, out result, collideWithRigidbodies: false))
        direction.y = 0.0f;
      CollisionData.Pool.Free(ref result);
      if ((double) direction.y < -0.0099999997764825821 && PhysicsEngine.Instance.RigidbodyCast(this.specRigidbody, IntVector2.Down, out result, collideWithRigidbodies: false))
        direction.y = 0.0f;
      CollisionData.Pool.Free(ref result);
      if (this.AcceptingNonMotionInput)
      {
        bool flag = (!this.IsPrimaryPlayer ? (int) GameManager.Options.additionalBlankControlTwo : (int) GameManager.Options.additionalBlankControl) == 1 && this.m_activeActions.CheckBothSticksButton();
        if ((double) UnityEngine.Time.timeScale > 0.0 && (this.m_activeActions.BlankAction.WasPressed || flag))
          this.DoConsumableBlank();
        if (BraveInput.GetInstanceForPlayer(this.PlayerIDX).GetButtonDown(GungeonActions.GungeonActionType.UseItem))
        {
          this.FireMissileVolley();
          BraveInput.GetInstanceForPlayer(this.PlayerIDX).ConsumeButtonDown(GungeonActions.GungeonActionType.UseItem);
        }
      }
      if (this.AcceptingNonMotionInput || this.CurrentInputState == PlayerInputState.FoyerInputOnly)
        this.m_aimAngle = BraveMathCollege.Atan2Degrees((Vector2) this.DetermineAimPointInWorld() - this.CenterPosition);
      if (this.m_isFiring && !BraveInput.GetInstanceForPlayer(this.PlayerIDX).GetButton(GungeonActions.GungeonActionType.Shoot))
      {
        this.m_isFiring = false;
        this.m_shouldContinueFiring = false;
      }
      if (this.SuppressThisClick)
      {
        while (BraveInput.GetInstanceForPlayer(this.PlayerIDX).GetButtonDown(GungeonActions.GungeonActionType.Shoot))
        {
          BraveInput.GetInstanceForPlayer(this.PlayerIDX).ConsumeButtonDown(GungeonActions.GungeonActionType.Shoot);
          if (BraveInput.GetInstanceForPlayer(this.PlayerIDX).GetButtonUp(GungeonActions.GungeonActionType.Shoot))
            BraveInput.GetInstanceForPlayer(this.PlayerIDX).ConsumeButtonUp(GungeonActions.GungeonActionType.Shoot);
        }
        if (!BraveInput.GetInstanceForPlayer(this.PlayerIDX).GetButton(GungeonActions.GungeonActionType.Shoot))
          this.SuppressThisClick = false;
      }
      else if (this.m_CanAttack && BraveInput.GetInstanceForPlayer(this.PlayerIDX).GetButtonDown(GungeonActions.GungeonActionType.Shoot))
      {
        bool flag1 = false;
        this.m_isFiring = true;
        bool flag2 = ((flag1 ? 1 : 0) | 1) != 0;
        this.m_shouldContinueFiring = true;
        if (flag2)
          BraveInput.GetInstanceForPlayer(this.PlayerIDX).ConsumeButtonDown(GungeonActions.GungeonActionType.Shoot);
      }
      else if (BraveInput.GetInstanceForPlayer(this.PlayerIDX).GetButtonUp(GungeonActions.GungeonActionType.Shoot))
      {
        this.m_isFiring = false;
        this.m_shouldContinueFiring = false;
        BraveInput.GetInstanceForPlayer(this.PlayerIDX).ConsumeButtonUp(GungeonActions.GungeonActionType.Shoot);
      }
      return direction;
    }
  }

