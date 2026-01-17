// Decompiled with JetBrains decompiler
// Type: TeleportBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using Pathfinding;
using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class TeleportBehavior : BasicAttackBehavior
    {
      public bool AttackableDuringAnimation;
      public bool AvoidWalls;
      public bool StayOnScreen = true;
      public float MinDistanceFromPlayer = 4f;
      public float MaxDistanceFromPlayer = -1f;
      public float GoneTime = 1f;
      [InspectorCategory("Conditions")]
      public bool OnlyTeleportIfPlayerUnreachable;
      [InspectorCategory("Attack")]
      public BulletScriptSelector teleportOutBulletScript;
      [InspectorCategory("Attack")]
      public BulletScriptSelector teleportInBulletScript;
      [InspectorCategory("Attack")]
      public AttackBehaviorBase goneAttackBehavior;
      [InspectorCategory("Attack")]
      public bool AllowCrossRoomTeleportation;
      [InspectorCategory("Visuals")]
      public string teleportOutAnim = "teleport_out";
      [InspectorCategory("Visuals")]
      public string teleportInAnim = "teleport_in";
      [InspectorCategory("Visuals")]
      public bool teleportRequiresTransparency;
      [InspectorCategory("Visuals")]
      public bool hasOutlinesDuringAnim = true;
      [InspectorCategory("Visuals")]
      public TeleportBehavior.ShadowSupport shadowSupport;
      [InspectorCategory("Visuals")]
      [InspectorShowIf("ShowShadowAnimationNames")]
      public string shadowOutAnim;
      [InspectorShowIf("ShowShadowAnimationNames")]
      [InspectorCategory("Visuals")]
      public string shadowInAnim;
      public bool ManuallyDefineRoom;
      [InspectorShowIf("ManuallyDefineRoom")]
      [InspectorIndent]
      public Vector2 roomMin;
      [InspectorShowIf("ManuallyDefineRoom")]
      [InspectorIndent]
      public Vector2 roomMax;
      private tk2dBaseSprite m_shadowSprite;
      private Shader m_cachedShader;
      private float m_timer;
      private bool m_shouldFire;
      private TeleportBehavior.TeleportState m_state;

      private bool ShowShadowAnimationNames()
      {
        return this.shadowSupport == TeleportBehavior.ShadowSupport.Animate;
      }

      public override void Start()
      {
        base.Start();
        this.m_aiActor.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
      }

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_timer);
        if (this.goneAttackBehavior == null)
          return;
        this.goneAttackBehavior.Upkeep();
      }

      public override bool IsReady()
      {
        return (!this.OnlyTeleportIfPlayerUnreachable || this.m_aiActor.GetAbsoluteParentRoom() != GameManager.Instance.BestActivePlayer.CurrentRoom || this.m_aiActor.Path == null || !this.m_aiActor.Path.WillReachFinalGoal) && base.IsReady();
      }

      public override BehaviorResult Update()
      {
        int num = (int) base.Update();
        if ((UnityEngine.Object) this.m_shadowSprite == (UnityEngine.Object) null)
          this.m_shadowSprite = this.m_aiActor.ShadowObject.GetComponent<tk2dBaseSprite>();
        if (!this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
          return BehaviorResult.Continue;
        this.State = TeleportBehavior.TeleportState.TeleportOut;
        this.m_updateEveryFrame = true;
        return BehaviorResult.RunContinuous;
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        int num = (int) base.ContinuousUpdate();
        if (this.State == TeleportBehavior.TeleportState.TeleportOut)
        {
          if (this.shadowSupport == TeleportBehavior.ShadowSupport.Fade)
            this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f - this.m_aiAnimator.CurrentClipProgress);
          if (!this.m_aiAnimator.IsPlaying(this.teleportOutAnim))
            this.State = TeleportBehavior.TeleportState.Gone;
        }
        else if (this.State == TeleportBehavior.TeleportState.Gone)
        {
          if ((double) this.m_timer <= 0.0)
            this.State = TeleportBehavior.TeleportState.GoneBehavior;
        }
        else if (this.State == TeleportBehavior.TeleportState.GoneBehavior)
        {
          if (this.goneAttackBehavior.ContinuousUpdate() == ContinuousBehaviorResult.Finished)
            this.State = TeleportBehavior.TeleportState.TeleportIn;
        }
        else if (this.State == TeleportBehavior.TeleportState.TeleportIn)
        {
          if (this.shadowSupport == TeleportBehavior.ShadowSupport.Fade)
            this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(this.m_aiAnimator.CurrentClipProgress);
          if ((bool) (UnityEngine.Object) this.m_aiShooter)
            this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (TeleportBehavior));
          if (!this.m_aiAnimator.IsPlaying(this.teleportInAnim))
          {
            this.State = TeleportBehavior.TeleportState.None;
            return ContinuousBehaviorResult.Finished;
          }
        }
        return ContinuousBehaviorResult.Continue;
      }

      public override void EndContinuousUpdate()
      {
        base.EndContinuousUpdate();
        if (this.teleportRequiresTransparency && (bool) (UnityEngine.Object) this.m_cachedShader)
        {
          this.m_aiActor.sprite.usesOverrideMaterial = false;
          this.m_aiActor.renderer.material.shader = this.m_cachedShader;
          this.m_cachedShader = (Shader) null;
        }
        this.m_aiActor.sprite.renderer.enabled = true;
        if ((bool) (UnityEngine.Object) this.m_aiActor.knockbackDoer)
          this.m_aiActor.knockbackDoer.SetImmobile(false, "teleport");
        this.m_aiActor.specRigidbody.CollideWithOthers = true;
        this.m_aiActor.IsGone = false;
        if ((bool) (UnityEngine.Object) this.m_aiShooter)
          this.m_aiShooter.ToggleGunAndHandRenderers(true, nameof (TeleportBehavior));
        if (!this.hasOutlinesDuringAnim)
          SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, true);
        if (this.goneAttackBehavior != null && this.State == TeleportBehavior.TeleportState.GoneBehavior)
          this.goneAttackBehavior.EndContinuousUpdate();
        this.m_aiAnimator.EndAnimationIf(this.teleportOutAnim);
        this.m_aiAnimator.EndAnimationIf(this.teleportInAnim);
        if (this.shadowSupport == TeleportBehavior.ShadowSupport.Fade)
          this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f);
        else if (this.shadowSupport == TeleportBehavior.ShadowSupport.Animate)
        {
          tk2dSpriteAnimationClip clipByName = this.m_shadowSprite.spriteAnimator.GetClipByName(this.shadowInAnim);
          this.m_shadowSprite.spriteAnimator.Play(clipByName, (float) (clipByName.frames.Length - 1), clipByName.fps);
        }
        this.m_state = TeleportBehavior.TeleportState.None;
        this.m_updateEveryFrame = false;
        this.UpdateCooldowns();
      }

      public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
      {
        base.Init(gameObject, aiActor, aiShooter);
        if (this.goneAttackBehavior == null)
          return;
        this.goneAttackBehavior.Init(gameObject, aiActor, aiShooter);
      }

      public override void SetDeltaTime(float deltaTime)
      {
        base.SetDeltaTime(deltaTime);
        if (this.goneAttackBehavior == null)
          return;
        this.goneAttackBehavior.SetDeltaTime(deltaTime);
      }

      public override bool UpdateEveryFrame()
      {
        return this.goneAttackBehavior != null && this.m_state == TeleportBehavior.TeleportState.GoneBehavior ? this.goneAttackBehavior.UpdateEveryFrame() : base.UpdateEveryFrame();
      }

      public void AnimationEventTriggered(
        tk2dSpriteAnimator animator,
        tk2dSpriteAnimationClip clip,
        int frame)
      {
        if (this.m_shouldFire && clip.GetFrame(frame).eventInfo == "fire")
        {
          if (this.State == TeleportBehavior.TeleportState.TeleportIn)
            SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.teleportInBulletScript);
          else if (this.State == TeleportBehavior.TeleportState.TeleportOut)
            SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.teleportOutBulletScript);
          this.m_shouldFire = false;
        }
        else
        {
          if (this.State != TeleportBehavior.TeleportState.TeleportOut || !(clip.GetFrame(frame).eventInfo == "teleport_collider_off"))
            return;
          this.m_aiActor.specRigidbody.CollideWithOthers = false;
          this.m_aiActor.IsGone = true;
        }
      }

      private TeleportBehavior.TeleportState State
      {
        get => this.m_state;
        set
        {
          this.EndState(this.m_state);
          this.m_state = value;
          this.BeginState(this.m_state);
        }
      }

      private void BeginState(TeleportBehavior.TeleportState state)
      {
        switch (state)
        {
          case TeleportBehavior.TeleportState.TeleportOut:
            if (this.teleportOutBulletScript != null && !this.teleportOutBulletScript.IsNull)
              this.m_shouldFire = true;
            if (this.teleportRequiresTransparency)
            {
              this.m_cachedShader = this.m_aiActor.renderer.material.shader;
              this.m_aiActor.sprite.usesOverrideMaterial = true;
              this.m_aiActor.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
            }
            this.m_aiAnimator.PlayUntilCancelled(this.teleportOutAnim, true);
            if (this.shadowSupport == TeleportBehavior.ShadowSupport.Animate)
              this.m_shadowSprite.spriteAnimator.PlayAndForceTime(this.shadowOutAnim, this.m_aiAnimator.CurrentClipLength);
            if ((bool) (UnityEngine.Object) this.m_aiActor.knockbackDoer)
              this.m_aiActor.knockbackDoer.SetImmobile(true, "teleport");
            this.m_aiActor.ClearPath();
            if (!this.AttackableDuringAnimation)
            {
              this.m_aiActor.specRigidbody.CollideWithOthers = false;
              this.m_aiActor.IsGone = true;
            }
            if ((bool) (UnityEngine.Object) this.m_aiShooter)
              this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (TeleportBehavior));
            if (this.hasOutlinesDuringAnim)
              break;
            SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, false);
            break;
          case TeleportBehavior.TeleportState.Gone:
            if ((double) this.GoneTime <= 0.0)
            {
              this.State = TeleportBehavior.TeleportState.GoneBehavior;
              break;
            }
            this.m_timer = this.GoneTime;
            this.m_aiActor.specRigidbody.CollideWithOthers = false;
            this.m_aiActor.IsGone = true;
            this.m_aiActor.sprite.renderer.enabled = false;
            break;
          default:
            if (this.State == TeleportBehavior.TeleportState.GoneBehavior)
            {
              if (this.goneAttackBehavior == null)
              {
                this.State = TeleportBehavior.TeleportState.TeleportIn;
                break;
              }
              switch (this.goneAttackBehavior.Update())
              {
                case BehaviorResult.RunContinuousInClass:
                  return;
                case BehaviorResult.RunContinuous:
                  return;
                default:
                  this.State = TeleportBehavior.TeleportState.TeleportIn;
                  return;
              }
            }
            else
            {
              if (state != TeleportBehavior.TeleportState.TeleportIn)
                break;
              if (this.teleportInBulletScript != null && !this.teleportInBulletScript.IsNull)
                this.m_shouldFire = true;
              this.DoTeleport();
              this.m_aiAnimator.PlayUntilFinished(this.teleportInAnim, true);
              if (this.shadowSupport == TeleportBehavior.ShadowSupport.Animate)
                this.m_shadowSprite.spriteAnimator.PlayAndForceTime(this.shadowInAnim, this.m_aiAnimator.CurrentClipLength);
              this.m_shadowSprite.renderer.enabled = true;
              if (this.AttackableDuringAnimation)
              {
                this.m_aiActor.specRigidbody.CollideWithOthers = true;
                this.m_aiActor.IsGone = false;
              }
              this.m_aiActor.sprite.renderer.enabled = true;
              if ((bool) (UnityEngine.Object) this.m_aiShooter)
                this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (TeleportBehavior));
              if (!this.hasOutlinesDuringAnim)
                break;
              SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, true);
              break;
            }
        }
      }

      private void EndState(TeleportBehavior.TeleportState state)
      {
        switch (state)
        {
          case TeleportBehavior.TeleportState.TeleportOut:
            this.m_shadowSprite.renderer.enabled = false;
            if (this.hasOutlinesDuringAnim)
              SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, false);
            if (this.teleportOutBulletScript == null || this.teleportOutBulletScript.IsNull || !this.m_shouldFire)
              break;
            SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.teleportOutBulletScript);
            this.m_shouldFire = false;
            break;
          case TeleportBehavior.TeleportState.TeleportIn:
            if (this.teleportRequiresTransparency && (bool) (UnityEngine.Object) this.m_cachedShader)
            {
              this.m_aiActor.sprite.usesOverrideMaterial = false;
              this.m_aiActor.renderer.material.shader = this.m_cachedShader;
              this.m_cachedShader = (Shader) null;
            }
            if (this.shadowSupport == TeleportBehavior.ShadowSupport.Fade)
              this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f);
            if ((bool) (UnityEngine.Object) this.m_aiActor.knockbackDoer)
              this.m_aiActor.knockbackDoer.SetImmobile(false, "teleport");
            this.m_aiActor.specRigidbody.CollideWithOthers = true;
            this.m_aiActor.IsGone = false;
            if ((bool) (UnityEngine.Object) this.m_aiShooter)
              this.m_aiShooter.ToggleGunAndHandRenderers(true, nameof (TeleportBehavior));
            if (this.teleportInBulletScript != null && !this.teleportInBulletScript.IsNull && this.m_shouldFire)
            {
              SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.teleportInBulletScript);
              this.m_shouldFire = false;
            }
            if (this.hasOutlinesDuringAnim)
              break;
            SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, true);
            break;
        }
      }

      private void DoTeleport()
      {
        float minDistanceFromPlayerSquared = this.MinDistanceFromPlayer * this.MinDistanceFromPlayer;
        float maxDistanceFromPlayerSquared = this.MaxDistanceFromPlayer * this.MaxDistanceFromPlayer;
        Vector2 playerLowerLeft = Vector2.zero;
        Vector2 playerUpperRight = Vector2.zero;
        bool hasOtherPlayer = false;
        Vector2 otherPlayerLowerLeft = Vector2.zero;
        Vector2 otherPlayerUpperRight = Vector2.zero;
        bool hasDistChecks = ((double) this.MinDistanceFromPlayer > 0.0 || (double) this.MaxDistanceFromPlayer > 0.0) && (bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody;
        if (hasDistChecks)
        {
          playerLowerLeft = this.m_aiActor.TargetRigidbody.HitboxPixelCollider.UnitBottomLeft;
          playerUpperRight = this.m_aiActor.TargetRigidbody.HitboxPixelCollider.UnitTopRight;
          PlayerController playerTarget = this.m_behaviorSpeculator.PlayerTarget as PlayerController;
          if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && (bool) (UnityEngine.Object) playerTarget)
          {
            PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(playerTarget);
            if ((bool) (UnityEngine.Object) otherPlayer && otherPlayer.healthHaver.IsAlive)
            {
              hasOtherPlayer = true;
              otherPlayerLowerLeft = otherPlayer.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
              otherPlayerUpperRight = otherPlayer.specRigidbody.HitboxPixelCollider.UnitTopRight;
            }
          }
        }
        IntVector2 bottomLeft = IntVector2.Zero;
        IntVector2 topRight = IntVector2.Zero;
        if (this.StayOnScreen)
        {
          bottomLeft = ((Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(0.0f, 0.0f), ViewportType.Gameplay)).ToIntVector2(VectorConversions.Ceil);
          topRight = ((Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(1f, 1f), ViewportType.Gameplay)).ToIntVector2(VectorConversions.Floor) - IntVector2.One;
        }
        CellValidator cellValidator = (CellValidator) (c =>
        {
          for (int index1 = 0; index1 < this.m_aiActor.Clearance.x; ++index1)
          {
            int x = c.x + index1;
            for (int index2 = 0; index2 < this.m_aiActor.Clearance.y; ++index2)
            {
              int y = c.y + index2;
              if (GameManager.Instance.Dungeon.data.isTopWall(x, y) || this.ManuallyDefineRoom && ((double) x < (double) this.roomMin.x || (double) x > (double) this.roomMax.x || (double) y < (double) this.roomMin.y || (double) y > (double) this.roomMax.y))
                return false;
            }
          }
          if (hasDistChecks)
          {
            PixelCollider hitboxPixelCollider = this.m_aiActor.specRigidbody.HitboxPixelCollider;
            Vector2 aMin = new Vector2((float) c.x + (float) (0.5 * ((double) this.m_aiActor.Clearance.x - (double) hitboxPixelCollider.UnitWidth)), (float) c.y);
            Vector2 aMax = aMin + hitboxPixelCollider.UnitDimensions;
            if ((double) this.MinDistanceFromPlayer > 0.0 && ((double) BraveMathCollege.AABBDistanceSquared(aMin, aMax, playerLowerLeft, playerUpperRight) < (double) minDistanceFromPlayerSquared || hasOtherPlayer && (double) BraveMathCollege.AABBDistanceSquared(aMin, aMax, otherPlayerLowerLeft, otherPlayerUpperRight) < (double) minDistanceFromPlayerSquared) || (double) this.MaxDistanceFromPlayer > 0.0 && ((double) BraveMathCollege.AABBDistanceSquared(aMin, aMax, playerLowerLeft, playerUpperRight) > (double) maxDistanceFromPlayerSquared || hasOtherPlayer && (double) BraveMathCollege.AABBDistanceSquared(aMin, aMax, otherPlayerLowerLeft, otherPlayerUpperRight) > (double) maxDistanceFromPlayerSquared))
              return false;
          }
          if (this.StayOnScreen && (c.x < bottomLeft.x || c.y < bottomLeft.y || c.x + this.m_aiActor.Clearance.x - 1 > topRight.x || c.y + this.m_aiActor.Clearance.y - 1 > topRight.y))
            return false;
          if (this.AvoidWalls)
          {
            int num1 = -1;
            for (int index = -1; index < this.m_aiActor.Clearance.y + 1; ++index)
            {
              if (GameManager.Instance.Dungeon.data.isWall(c.x + num1, c.y + index))
                return false;
            }
            int x = this.m_aiActor.Clearance.x;
            for (int index = -1; index < this.m_aiActor.Clearance.y + 1; ++index)
            {
              if (GameManager.Instance.Dungeon.data.isWall(c.x + x, c.y + index))
                return false;
            }
            int num2 = -1;
            for (int index = -1; index < this.m_aiActor.Clearance.x + 1; ++index)
            {
              if (GameManager.Instance.Dungeon.data.isWall(c.x + index, c.y + num2))
                return false;
            }
            int y = this.m_aiActor.Clearance.y;
            for (int index = -1; index < this.m_aiActor.Clearance.x + 1; ++index)
            {
              if (GameManager.Instance.Dungeon.data.isWall(c.x + index, c.y + y))
                return false;
            }
          }
          return true;
        });
        Vector2 vector2 = this.m_aiActor.specRigidbody.UnitBottomCenter - this.m_aiActor.transform.position.XY();
        IntVector2? nullable1 = new IntVector2?();
        IntVector2? nullable2 = !this.AllowCrossRoomTeleportation ? this.m_aiActor.ParentRoom.GetRandomAvailableCell(new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), cellValidator: cellValidator) : GameManager.Instance.BestActivePlayer.CurrentRoom.GetRandomAvailableCell(new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), cellValidator: cellValidator);
        if (nullable2.HasValue)
        {
          this.m_aiActor.transform.position = (Vector3) (Pathfinder.GetClearanceOffset(nullable2.Value, this.m_aiActor.Clearance).WithY((float) nullable2.Value.y) - vector2);
          this.m_aiActor.specRigidbody.Reinitialize();
        }
        else
          Debug.LogWarning((object) "TELEPORT FAILED!", (UnityEngine.Object) this.m_aiActor);
      }

      public enum ShadowSupport
      {
        None,
        Fade,
        Animate,
      }

      private enum TeleportState
      {
        None,
        TeleportOut,
        Gone,
        GoneBehavior,
        TeleportIn,
      }
    }

}
