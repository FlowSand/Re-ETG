// Decompiled with JetBrains decompiler
// Type: BossFinalMarinePortalBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using Pathfinding;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    [InspectorDropdownName("Bosses/BossFinalMarine/PortalBehavior")]
    public class BossFinalMarinePortalBehavior : BasicAttackBehavior
    {
      public bool AttackableDuringAnimation;
      public float GoneTime = 1f;
      public float PortalSize = 16f;
      [InspectorCategory("Visuals")]
      public string teleportOutAnim = "teleport_out";
      [InspectorCategory("Visuals")]
      public string teleportInAnim = "teleport_in";
      [InspectorCategory("Visuals")]
      public bool teleportRequiresTransparency;
      [InspectorCategory("Visuals")]
      public bool hasOutlinesDuringAnim = true;
      [InspectorCategory("Visuals")]
      public string portalAnim = "weak_attack_charge";
      [InspectorCategory("Visuals")]
      public BossFinalMarinePortalBehavior.ShadowSupport shadowSupport;
      [InspectorShowIf("ShowShadowAnimationNames")]
      [InspectorCategory("Visuals")]
      public string shadowOutAnim;
      [InspectorCategory("Visuals")]
      [InspectorShowIf("ShowShadowAnimationNames")]
      public string shadowInAnim;
      public bool ManuallyDefineRoom;
      [InspectorShowIf("ManuallyDefineRoom")]
      [InspectorIndent]
      public Vector2 roomMin;
      [InspectorIndent]
      [InspectorShowIf("ManuallyDefineRoom")]
      public Vector2 roomMax;
      private DimensionFogController m_portalController;
      private tk2dBaseSprite m_shadowSprite;
      private Shader m_cachedShader;
      private float m_timer;
      private BossFinalMarinePortalBehavior.TeleportState m_state;

      private bool ShowShadowAnimationNames()
      {
        return this.shadowSupport == BossFinalMarinePortalBehavior.ShadowSupport.Animate;
      }

      public override void Start()
      {
        base.Start();
        this.m_portalController = Object.FindObjectOfType<DimensionFogController>();
      }

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_timer);
      }

      public override BehaviorResult Update()
      {
        int num = (int) base.Update();
        if ((Object) this.m_shadowSprite == (Object) null)
          this.m_shadowSprite = this.m_aiActor.ShadowObject.GetComponent<tk2dBaseSprite>();
        if (!this.IsReady() || !(bool) (Object) this.m_aiActor.TargetRigidbody)
          return BehaviorResult.Continue;
        this.State = BossFinalMarinePortalBehavior.TeleportState.TeleportOut;
        this.m_updateEveryFrame = true;
        return BehaviorResult.RunContinuous;
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        int num = (int) base.ContinuousUpdate();
        if (this.State == BossFinalMarinePortalBehavior.TeleportState.TeleportOut)
        {
          if (this.shadowSupport == BossFinalMarinePortalBehavior.ShadowSupport.Fade)
            this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f - this.m_aiAnimator.CurrentClipProgress);
          if (!this.m_aiAnimator.IsPlaying(this.teleportOutAnim))
            this.State = BossFinalMarinePortalBehavior.TeleportState.Gone;
        }
        else if (this.State == BossFinalMarinePortalBehavior.TeleportState.Gone)
        {
          if ((double) this.m_timer <= 0.0)
            this.State = BossFinalMarinePortalBehavior.TeleportState.TeleportIn;
        }
        else if (this.State == BossFinalMarinePortalBehavior.TeleportState.TeleportIn)
        {
          if (this.shadowSupport == BossFinalMarinePortalBehavior.ShadowSupport.Fade)
            this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(this.m_aiAnimator.CurrentClipProgress);
          if ((bool) (Object) this.m_aiShooter)
            this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (BossFinalMarinePortalBehavior));
          if (!this.m_aiAnimator.IsPlaying(this.teleportInAnim))
            this.State = BossFinalMarinePortalBehavior.TeleportState.PostTeleport;
        }
        else if (this.State == BossFinalMarinePortalBehavior.TeleportState.PostTeleport && !this.m_aiAnimator.IsPlaying(this.portalAnim))
        {
          this.State = BossFinalMarinePortalBehavior.TeleportState.None;
          return ContinuousBehaviorResult.Finished;
        }
        return ContinuousBehaviorResult.Continue;
      }

      public override void EndContinuousUpdate()
      {
        base.EndContinuousUpdate();
        this.m_updateEveryFrame = false;
        this.UpdateCooldowns();
      }

      public override bool IsOverridable() => false;

      private BossFinalMarinePortalBehavior.TeleportState State
      {
        get => this.m_state;
        set
        {
          this.EndState(this.m_state);
          this.m_state = value;
          this.BeginState(this.m_state);
        }
      }

      private void BeginState(BossFinalMarinePortalBehavior.TeleportState state)
      {
        switch (state)
        {
          case BossFinalMarinePortalBehavior.TeleportState.TeleportOut:
            if (this.teleportRequiresTransparency)
            {
              this.m_cachedShader = this.m_aiActor.renderer.material.shader;
              this.m_aiActor.sprite.usesOverrideMaterial = true;
              this.m_aiActor.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
            }
            this.m_aiAnimator.PlayUntilCancelled(this.teleportOutAnim, true);
            if (this.shadowSupport == BossFinalMarinePortalBehavior.ShadowSupport.Animate)
              this.m_shadowSprite.spriteAnimator.PlayAndForceTime(this.shadowOutAnim, this.m_aiAnimator.CurrentClipLength);
            this.m_aiActor.ClearPath();
            if (!this.AttackableDuringAnimation)
            {
              this.m_aiActor.specRigidbody.CollideWithOthers = false;
              this.m_aiActor.IsGone = true;
            }
            if ((bool) (Object) this.m_aiShooter)
              this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (BossFinalMarinePortalBehavior));
            if (this.hasOutlinesDuringAnim)
              break;
            SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, false);
            break;
          case BossFinalMarinePortalBehavior.TeleportState.Gone:
            if ((double) this.GoneTime <= 0.0)
            {
              this.State = BossFinalMarinePortalBehavior.TeleportState.TeleportIn;
              break;
            }
            this.m_timer = this.GoneTime;
            this.m_aiActor.specRigidbody.CollideWithOthers = false;
            this.m_aiActor.IsGone = true;
            this.m_aiActor.sprite.renderer.enabled = false;
            break;
          case BossFinalMarinePortalBehavior.TeleportState.TeleportIn:
            this.DoTeleport();
            this.m_aiAnimator.PlayUntilFinished(this.teleportInAnim, true);
            if (this.shadowSupport == BossFinalMarinePortalBehavior.ShadowSupport.Animate)
              this.m_shadowSprite.spriteAnimator.PlayAndForceTime(this.shadowInAnim, this.m_aiAnimator.CurrentClipLength);
            this.m_shadowSprite.renderer.enabled = true;
            if (this.AttackableDuringAnimation)
            {
              this.m_aiActor.specRigidbody.CollideWithOthers = true;
              this.m_aiActor.IsGone = false;
            }
            this.m_aiActor.sprite.renderer.enabled = true;
            if ((bool) (Object) this.m_aiShooter)
              this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (BossFinalMarinePortalBehavior));
            if (!this.hasOutlinesDuringAnim)
              break;
            SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, true);
            break;
          case BossFinalMarinePortalBehavior.TeleportState.PostTeleport:
            this.m_aiAnimator.PlayUntilFinished(this.portalAnim, true);
            this.m_portalController.targetRadius = this.PortalSize;
            break;
        }
      }

      private void EndState(BossFinalMarinePortalBehavior.TeleportState state)
      {
        switch (state)
        {
          case BossFinalMarinePortalBehavior.TeleportState.TeleportOut:
            this.m_shadowSprite.renderer.enabled = false;
            if (!this.hasOutlinesDuringAnim)
              break;
            SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, false);
            break;
          case BossFinalMarinePortalBehavior.TeleportState.TeleportIn:
            if (this.teleportRequiresTransparency)
            {
              this.m_aiActor.sprite.usesOverrideMaterial = false;
              this.m_aiActor.renderer.material.shader = this.m_cachedShader;
            }
            if (this.shadowSupport == BossFinalMarinePortalBehavior.ShadowSupport.Fade)
              this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f);
            this.m_aiActor.specRigidbody.CollideWithOthers = true;
            this.m_aiActor.IsGone = false;
            if ((bool) (Object) this.m_aiShooter)
              this.m_aiShooter.ToggleGunAndHandRenderers(true, nameof (BossFinalMarinePortalBehavior));
            if (this.hasOutlinesDuringAnim)
              break;
            SpriteOutlineManager.ToggleOutlineRenderers(this.m_aiActor.sprite, true);
            break;
        }
      }

      private void DoTeleport()
      {
        Vector2 vector2 = this.m_aiActor.specRigidbody.UnitCenter - this.m_aiActor.transform.position.XY();
        this.m_aiActor.transform.position = (Vector3) (Pathfinder.GetClearanceOffset((this.roomMin + (this.roomMax - this.roomMin + Vector2.one) / 2f).ToIntVector2(VectorConversions.Floor), this.m_aiActor.Clearance) - vector2);
        this.m_aiActor.specRigidbody.Reinitialize();
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
        TeleportIn,
        PostTeleport,
      }
    }

}
