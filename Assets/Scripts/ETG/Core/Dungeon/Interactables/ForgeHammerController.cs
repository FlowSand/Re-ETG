using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class ForgeHammerController : DungeonPlaceableBehaviour, IPlaceConfigurable
  {
    [DwarfConfigurable]
    public bool TracksPlayer = true;
    [DwarfConfigurable]
    public bool DeactivateOnEnemiesCleared = true;
    [DwarfConfigurable]
    public bool ForceLeft;
    [DwarfConfigurable]
    public bool ForceRight;
    public float FlashDurationBeforeAttack = 0.5f;
    public float AdditionalTrackingTime = 0.25f;
    public float DamageToEnemies = 30f;
    public float KnockbackForcePlayers = 50f;
    public float KnockbackForceEnemies = 50f;
    [DwarfConfigurable]
    public float InitialDelay = 1f;
    [DwarfConfigurable]
    public float MinTimeBetweenAttacks = 2f;
    [DwarfConfigurable]
    public float MaxTimeBetweenAttacks = 4f;
    [DwarfConfigurable]
    public float MinTimeToRestOnGround = 1f;
    [DwarfConfigurable]
    public float MaxTimeToRestOnGround = 1f;
    public bool DoScreenShake;
    public ScreenShakeSettings ScreenShake;
    public string Hammer_Anim_In_Left;
    public string Hammer_Anim_Out_Left;
    public string Hammer_Anim_In_Right;
    public string Hammer_Anim_Out_Right;
    public tk2dSpriteAnimator HitEffectAnimator;
    public tk2dSpriteAnimator TargetAnimator;
    public tk2dSpriteAnimator ShadowAnimator;
    public bool DoGoopOnImpact;
    [ShowInInspectorIf("DoGoopOnImpact", false)]
    public GoopDefinition GoopToDo;
    [DwarfConfigurable]
    public bool DoesBulletsOnImpact;
    [ShowInInspectorIf("DoGoopOnImpact", false)]
    public BulletScriptSelector BulletScript;
    [ShowInInspectorIf("DoGoopOnImpact", false)]
    public Transform ShootPoint;
    private float m_localTimeScale = 1f;
    private ForgeHammerController.HammerState m_state = ForgeHammerController.HammerState.Gone;
    private float m_timer;
    private PlayerController m_targetPlayer;
    private Vector2 m_targetOffset;
    private string m_inAnim;
    private string m_outAnim;
    private float m_additionalTrackTimer;
    private RoomHandler m_room;
    private bool m_isActive;
    private BulletScriptSource m_bulletSource;
    private bool m_attackIsLeft;

    private float LocalDeltaTime => BraveTime.DeltaTime * this.LocalTimeScale;

    public float LocalTimeScale
    {
      get => this.m_localTimeScale;
      set
      {
        this.spriteAnimator.OverrideTimeScale = value;
        this.m_localTimeScale = value;
      }
    }

    public void Start()
    {
      PhysicsEngine.Instance.OnPostRigidbodyMovement += new System.Action(this.OnPostRigidbodyMovement);
    }

    public void Update()
    {
      if (!this.m_isActive && this.State == ForgeHammerController.HammerState.Gone)
        return;
      if (this.m_isActive && this.State == ForgeHammerController.HammerState.Gone)
      {
        Vector2 unitBottomLeft = this.m_room.area.UnitBottomLeft;
        Vector2 unitTopRight = this.m_room.area.UnitTopRight;
        int num = 0;
        for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
        {
          PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
          if ((bool) (UnityEngine.Object) allPlayer && allPlayer.healthHaver.IsAlive)
          {
            Vector2 centerPosition = allPlayer.CenterPosition;
            if (BraveMathCollege.AABBContains(unitBottomLeft - Vector2.one, unitTopRight + Vector2.one, centerPosition))
              ++num;
          }
        }
        if (num == 0)
          this.Deactivate();
      }
      this.m_timer = Mathf.Max(0.0f, this.m_timer - this.LocalDeltaTime);
      this.UpdateState(this.State);
    }

    protected override void OnDestroy()
    {
      StaticReferenceManager.AllForgeHammers.Remove(this);
      base.OnDestroy();
    }

    public void Activate()
    {
      if (this.m_isActive)
        return;
      if (this.DeactivateOnEnemiesCleared && !this.m_room.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
      {
        this.ForceStop();
      }
      else
      {
        this.m_isActive = true;
        if (this.State != ForgeHammerController.HammerState.Gone)
          return;
        this.State = ForgeHammerController.HammerState.InitialDelay;
      }
    }

    public void Deactivate()
    {
      if (!this.m_isActive)
        return;
      if ((bool) (UnityEngine.Object) this.encounterTrackable)
        GameStatsManager.Instance.HandleEncounteredObject(this.encounterTrackable);
      this.m_isActive = false;
    }

    private ForgeHammerController.HammerState State
    {
      get => this.m_state;
      set
      {
        if (value == this.m_state)
          return;
        this.EndState(this.m_state);
        this.m_state = value;
        this.BeginState(this.m_state);
      }
    }

    private void BeginState(ForgeHammerController.HammerState state)
    {
      switch (state)
      {
        case ForgeHammerController.HammerState.InitialDelay:
          this.TargetAnimator.renderer.enabled = false;
          this.HitEffectAnimator.renderer.enabled = false;
          this.sprite.renderer.enabled = false;
          this.m_timer = this.InitialDelay;
          break;
        case ForgeHammerController.HammerState.PreSwing:
          this.m_targetPlayer = GameManager.Instance.GetRandomActivePlayer();
          if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
          {
            List<PlayerController> list = new List<PlayerController>(2);
            Vector2 unitBottomLeft = this.m_room.area.UnitBottomLeft;
            Vector2 unitTopRight = this.m_room.area.UnitTopRight;
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
              PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
              if ((bool) (UnityEngine.Object) allPlayer && allPlayer.healthHaver.IsAlive)
              {
                Vector2 centerPosition = allPlayer.CenterPosition;
                if (BraveMathCollege.AABBContains(unitBottomLeft - Vector2.one, unitTopRight + Vector2.one, centerPosition))
                  list.Add(allPlayer);
              }
            }
            if (list.Count > 0)
              this.m_targetPlayer = BraveUtility.RandomElement<PlayerController>(list);
          }
          IntVector2 intVector2 = this.m_targetPlayer.CenterPosition.ToIntVector2(VectorConversions.Floor);
          if (this.ForceLeft)
            this.m_attackIsLeft = true;
          else if (this.ForceRight)
          {
            this.m_attackIsLeft = false;
          }
          else
          {
            int num1 = 0;
            while (GameManager.Instance.Dungeon.data[intVector2 + IntVector2.Left * num1].type != CellType.WALL)
              ++num1;
            int num2 = 0;
            while (GameManager.Instance.Dungeon.data[intVector2 + IntVector2.Right * num2].type != CellType.WALL)
              ++num2;
            this.m_attackIsLeft = num1 < num2;
          }
          this.m_inAnim = !this.m_attackIsLeft ? this.Hammer_Anim_In_Right : this.Hammer_Anim_In_Left;
          this.m_outAnim = !this.m_attackIsLeft ? this.Hammer_Anim_Out_Right : this.Hammer_Anim_Out_Left;
          this.TargetAnimator.StopAndResetFrame();
          this.TargetAnimator.renderer.enabled = this.TracksPlayer;
          this.TargetAnimator.PlayAndDisableRenderer(!this.m_attackIsLeft ? "hammer_right_target" : "hammer_left_target");
          this.m_targetOffset = !this.m_attackIsLeft ? new Vector2(4.625f, 31f / 16f) : new Vector2(31f / 16f, 31f / 16f);
          this.m_timer = this.FlashDurationBeforeAttack;
          break;
        case ForgeHammerController.HammerState.Swing:
          this.sprite.renderer.enabled = true;
          this.spriteAnimator.Play(this.m_inAnim);
          this.ShadowAnimator.renderer.enabled = true;
          this.ShadowAnimator.Play(!this.m_attackIsLeft ? "hammer_right_slam_shadow" : "hammer_left_slam_shadow");
          this.sprite.HeightOffGround = -2.5f;
          this.sprite.UpdateZDepth();
          this.m_additionalTrackTimer = this.AdditionalTrackingTime;
          break;
        case ForgeHammerController.HammerState.Grounded:
          if (this.DoScreenShake)
            GameManager.Instance.MainCameraController.DoScreenShake(this.ScreenShake, new Vector2?(this.specRigidbody.UnitCenter));
          this.specRigidbody.enabled = true;
          this.specRigidbody.PixelColliders[0].ManualOffsetX = !this.m_attackIsLeft ? 59 : 16 /*0x10*/;
          this.specRigidbody.PixelColliders[1].ManualOffsetX = !this.m_attackIsLeft ? 59 : 16 /*0x10*/;
          this.specRigidbody.ForceRegenerate();
          this.specRigidbody.Reinitialize();
          Exploder.DoRadialMinorBreakableBreak((Vector3) this.TargetAnimator.sprite.WorldCenter, 4f);
          this.HitEffectAnimator.renderer.enabled = true;
          this.HitEffectAnimator.PlayAndDisableRenderer(!this.m_attackIsLeft ? "hammer_right_slam_vfx" : "hammer_left_slam_vfx");
          List<SpeculativeRigidbody> overlappingRigidbodies = PhysicsEngine.Instance.GetOverlappingRigidbodies(this.specRigidbody);
          for (int index = 0; index < overlappingRigidbodies.Count; ++index)
          {
            if ((bool) (UnityEngine.Object) overlappingRigidbodies[index].gameActor)
            {
              Vector2 direction = overlappingRigidbodies[index].UnitCenter - this.specRigidbody.UnitCenter;
              if (overlappingRigidbodies[index].gameActor is PlayerController)
              {
                PlayerController gameActor = overlappingRigidbodies[index].gameActor as PlayerController;
                if (overlappingRigidbodies[index].CollideWithOthers && (!gameActor.DodgeRollIsBlink || !gameActor.IsDodgeRolling))
                {
                  if ((bool) (UnityEngine.Object) overlappingRigidbodies[index].healthHaver)
                    overlappingRigidbodies[index].healthHaver.ApplyDamage(0.5f, direction, StringTableManager.GetEnemiesString("#FORGE_HAMMER"));
                  if ((bool) (UnityEngine.Object) overlappingRigidbodies[index].knockbackDoer)
                    overlappingRigidbodies[index].knockbackDoer.ApplyKnockback(direction, this.KnockbackForcePlayers);
                }
              }
              else
              {
                if ((bool) (UnityEngine.Object) overlappingRigidbodies[index].healthHaver)
                  overlappingRigidbodies[index].healthHaver.ApplyDamage(this.DamageToEnemies, direction, StringTableManager.GetEnemiesString("#FORGE_HAMMER"));
                if ((bool) (UnityEngine.Object) overlappingRigidbodies[index].knockbackDoer)
                  overlappingRigidbodies[index].knockbackDoer.ApplyKnockback(direction, this.KnockbackForceEnemies);
              }
            }
          }
          PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.specRigidbody);
          if (this.DoGoopOnImpact)
            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.GoopToDo).AddGoopRect(this.specRigidbody.UnitCenter + new Vector2(-1f, -1.25f), this.specRigidbody.UnitCenter + new Vector2(1f, 0.75f));
          if (this.DoesBulletsOnImpact && this.m_isActive)
          {
            this.ShootPoint.transform.position = (Vector3) this.specRigidbody.UnitCenter;
            CellData cell = this.ShootPoint.transform.position.GetCell();
            if (cell != null && cell.type != CellType.WALL)
            {
              if (!(bool) (UnityEngine.Object) this.m_bulletSource)
                this.m_bulletSource = this.ShootPoint.gameObject.GetOrAddComponent<BulletScriptSource>();
              this.m_bulletSource.BulletManager = this.bulletBank;
              this.m_bulletSource.BulletScript = this.BulletScript;
              this.m_bulletSource.Initialize();
            }
          }
          this.m_timer = UnityEngine.Random.Range(this.MinTimeToRestOnGround, this.MaxTimeToRestOnGround);
          break;
        case ForgeHammerController.HammerState.UpSwing:
          this.spriteAnimator.Play(this.m_outAnim);
          this.ShadowAnimator.PlayAndDisableRenderer(!this.m_attackIsLeft ? "hammer_right_out_shadow" : "hammer_left_out_shadow");
          break;
        case ForgeHammerController.HammerState.Gone:
          this.sprite.renderer.enabled = false;
          this.m_timer = UnityEngine.Random.Range(this.MinTimeBetweenAttacks, this.MaxTimeBetweenAttacks);
          break;
      }
    }

    private void UpdateState(ForgeHammerController.HammerState state)
    {
      switch (state)
      {
        case ForgeHammerController.HammerState.InitialDelay:
          if ((double) this.m_timer > 0.0)
            break;
          this.State = ForgeHammerController.HammerState.PreSwing;
          break;
        case ForgeHammerController.HammerState.PreSwing:
          if ((double) this.m_timer > 0.0)
            break;
          this.State = ForgeHammerController.HammerState.Swing;
          break;
        case ForgeHammerController.HammerState.Swing:
          this.m_additionalTrackTimer -= this.LocalDeltaTime;
          if (this.spriteAnimator.IsPlaying(this.m_inAnim))
            break;
          this.State = ForgeHammerController.HammerState.Grounded;
          break;
        case ForgeHammerController.HammerState.Grounded:
          if ((double) this.m_timer > 0.0)
            break;
          this.State = ForgeHammerController.HammerState.UpSwing;
          break;
        case ForgeHammerController.HammerState.UpSwing:
          if (this.spriteAnimator.IsPlaying(this.m_outAnim))
            break;
          this.State = ForgeHammerController.HammerState.Gone;
          break;
        case ForgeHammerController.HammerState.Gone:
          if ((double) this.m_timer > 0.0)
            break;
          this.State = ForgeHammerController.HammerState.PreSwing;
          break;
      }
    }

    private void EndState(ForgeHammerController.HammerState state)
    {
      if (state != ForgeHammerController.HammerState.Grounded)
        return;
      this.specRigidbody.enabled = false;
    }

    public void ConfigureOnPlacement(RoomHandler room)
    {
      StaticReferenceManager.AllForgeHammers.Add(this);
      this.m_room = room;
      if (room.visibility == RoomHandler.VisibilityStatus.CURRENT)
        this.DoRealConfigure(true);
      else
        this.StartCoroutine(this.FrameDelayedConfigure());
    }

    [DebuggerHidden]
    private IEnumerator FrameDelayedConfigure()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ForgeHammerController__FrameDelayedConfigurec__Iterator0()
      {
        _this = this
      };
    }

    private void DoRealConfigure(bool activateNow)
    {
      if (this.ForceLeft)
        this.transform.position += new Vector3(-1f, -1f, 0.0f);
      else if (this.ForceRight)
        this.transform.position += new Vector3(-57f / 16f, -1f, 0.0f);
      this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
      this.m_room.Entered += (RoomHandler.OnEnteredEventHandler) (a => this.Activate());
      if (activateNow)
        this.Activate();
      if (!this.DeactivateOnEnemiesCleared)
        return;
      this.m_room.OnEnemiesCleared += new System.Action(this.Deactivate);
    }

    private void HandleAnimationEvent(
      tk2dSpriteAnimator sourceAnimator,
      tk2dSpriteAnimationClip sourceClip,
      int sourceFrame)
    {
      if (this.State != ForgeHammerController.HammerState.Swing || !(sourceClip.frames[sourceFrame].eventInfo == "impact"))
        return;
      this.State = ForgeHammerController.HammerState.Grounded;
    }

    private void OnPostRigidbodyMovement()
    {
      if (!this.TracksPlayer || this.State != ForgeHammerController.HammerState.PreSwing && ((double) this.m_additionalTrackTimer <= 0.0 || this.State != ForgeHammerController.HammerState.Swing))
        return;
      this.UpdatePosition();
    }

    private void UpdatePosition()
    {
      this.transform.position = (Vector3) (BraveMathCollege.ClampToBounds(this.m_targetPlayer.CenterPosition, this.m_room.area.UnitBottomLeft + Vector2.one, this.m_room.area.UnitTopRight - Vector2.one) - this.m_targetOffset).Quantize(1f / 16f);
      this.TargetAnimator.sprite.UpdateZDepth();
      this.sprite.UpdateZDepth();
    }

    private void ForceStop()
    {
      if ((bool) (UnityEngine.Object) this.TargetAnimator)
        this.TargetAnimator.renderer.enabled = false;
      if ((bool) (UnityEngine.Object) this.HitEffectAnimator)
        this.HitEffectAnimator.renderer.enabled = false;
      if ((bool) (UnityEngine.Object) this.sprite)
        this.sprite.renderer.enabled = false;
      if ((bool) (UnityEngine.Object) this.ShadowAnimator)
        this.ShadowAnimator.renderer.enabled = false;
      this.specRigidbody.enabled = false;
      if ((bool) (UnityEngine.Object) this.m_bulletSource)
        this.m_bulletSource.ForceStop();
      this.State = ForgeHammerController.HammerState.Gone;
    }

    private enum HammerState
    {
      InitialDelay,
      PreSwing,
      Swing,
      Grounded,
      UpSwing,
      Gone,
    }
  }

