// Decompiled with JetBrains decompiler
// Type: TransformBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System;
using UnityEngine;

#nullable disable

public class TransformBehavior : BasicAttackBehavior
  {
    public TransformBehavior.Invulnerability invulnerabilityMode = TransformBehavior.Invulnerability.None;
    public bool reflectBullets;
    public float transformedTime = 1f;
    public bool goneWhileTransformed;
    public bool Uninterruptible;
    [InspectorCategory("Attack")]
    public GameObject shootPoint;
    [InspectorCategory("Attack")]
    public BulletScriptSelector inBulletScript;
    [InspectorCategory("Attack")]
    public BulletScriptSelector transformedBulletScript;
    [InspectorCategory("Attack")]
    public bool transformFireImmediately;
    [InspectorCategory("Attack")]
    public BulletScriptSelector outBulletScript;
    [InspectorCategory("Visuals")]
    public string inAnim;
    [InspectorCategory("Visuals")]
    public string transformedAnim;
    [InspectorCategory("Visuals")]
    public bool setTransformAnimAsBaseState;
    [InspectorCategory("Visuals")]
    public string outAnim;
    [InspectorCategory("Visuals")]
    public bool requiresTransparency;
    [InspectorCategory("Visuals")]
    public TransformBehavior.ShadowSupport shadowSupport = TransformBehavior.ShadowSupport.None;
    [InspectorCategory("Visuals")]
    [InspectorShowIf("ShowShadowAnimationNames")]
    public string shadowInAnim;
    [InspectorShowIf("ShowShadowAnimationNames")]
    [InspectorCategory("Visuals")]
    public string shadowOutAnim;
    private tk2dBaseSprite m_shadowSprite;
    private Shader m_cachedShader;
    private BulletScriptSource m_bulletSource;
    private PixelCollider m_enemyHitbox;
    private PixelCollider m_bulletBlocker;
    private float m_timer;
    private bool m_shouldFire;
    private bool m_isInvulnerable;
    private bool m_hasTransitioned;
    private TransformBehavior.TransformState m_state;

    private bool ShowShadowAnimationNames()
    {
      return this.shadowSupport == TransformBehavior.ShadowSupport.Animate;
    }

    private bool ShowReflectBullets()
    {
      return this.invulnerabilityMode != TransformBehavior.Invulnerability.None;
    }

    public override void Start()
    {
      base.Start();
      this.m_aiActor.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
      if (this.invulnerabilityMode == TransformBehavior.Invulnerability.None)
        return;
      for (int index = 0; index < this.m_aiActor.specRigidbody.PixelColliders.Count; ++index)
      {
        PixelCollider pixelCollider = this.m_aiActor.specRigidbody.PixelColliders[index];
        if (pixelCollider.CollisionLayer == CollisionLayer.EnemyHitBox)
          this.m_enemyHitbox = pixelCollider;
        if (pixelCollider.CollisionLayer == CollisionLayer.BulletBlocker)
          this.m_bulletBlocker = pixelCollider;
      }
      if (this.invulnerabilityMode == TransformBehavior.Invulnerability.WhileTransformed)
      {
        this.Invulnerable = false;
      }
      else
      {
        if (this.invulnerabilityMode != TransformBehavior.Invulnerability.WhileNotTransformed)
          return;
        this.Invulnerable = true;
      }
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_timer);
    }

    public override BehaviorResult Update()
    {
      int num = (int) base.Update();
      if ((UnityEngine.Object) this.m_shadowSprite == (UnityEngine.Object) null)
        this.m_shadowSprite = this.m_aiActor.ShadowObject.GetComponent<tk2dBaseSprite>();
      if (!this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
        return BehaviorResult.Continue;
      this.State = TransformBehavior.TransformState.InTrans;
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      if (this.State == TransformBehavior.TransformState.InTrans)
      {
        if (this.shadowSupport == TransformBehavior.ShadowSupport.Fade)
          this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f - this.m_aiAnimator.CurrentClipProgress);
        if (!this.m_hasTransitioned && (double) this.m_aiAnimator.CurrentClipProgress > 0.5)
        {
          if (this.invulnerabilityMode == TransformBehavior.Invulnerability.WhileTransformed)
            this.Invulnerable = true;
          else if (this.invulnerabilityMode == TransformBehavior.Invulnerability.WhileNotTransformed)
            this.Invulnerable = false;
          this.m_hasTransitioned = true;
        }
        if (!this.m_aiAnimator.IsPlaying(this.inAnim))
          this.State = (double) this.transformedTime <= 0.0 ? TransformBehavior.TransformState.OutTrans : TransformBehavior.TransformState.Transformed;
      }
      else if (this.State == TransformBehavior.TransformState.Transformed)
      {
        if ((double) this.m_timer <= 0.0)
          this.State = TransformBehavior.TransformState.OutTrans;
      }
      else if (this.State == TransformBehavior.TransformState.OutTrans)
      {
        if (this.shadowSupport == TransformBehavior.ShadowSupport.Fade)
          this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(this.m_aiAnimator.CurrentClipProgress);
        if (!this.m_hasTransitioned && (double) this.m_aiAnimator.CurrentClipProgress > 0.5)
        {
          if (this.invulnerabilityMode == TransformBehavior.Invulnerability.WhileTransformed)
            this.Invulnerable = false;
          else if (this.invulnerabilityMode == TransformBehavior.Invulnerability.WhileNotTransformed)
            this.Invulnerable = true;
          this.m_hasTransitioned = true;
        }
        if ((bool) (UnityEngine.Object) this.m_aiShooter)
          this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (TransformBehavior));
        if (!this.m_aiAnimator.IsPlaying(this.outAnim))
        {
          this.State = TransformBehavior.TransformState.None;
          return ContinuousBehaviorResult.Finished;
        }
      }
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      if (this.invulnerabilityMode != TransformBehavior.Invulnerability.None)
      {
        if (this.invulnerabilityMode == TransformBehavior.Invulnerability.WhileTransformed)
          this.Invulnerable = false;
        else if (this.invulnerabilityMode == TransformBehavior.Invulnerability.WhileNotTransformed)
          this.Invulnerable = true;
      }
      if ((bool) (UnityEngine.Object) this.m_aiShooter)
        this.m_aiShooter.ToggleGunAndHandRenderers(true, nameof (TransformBehavior));
      if ((bool) (UnityEngine.Object) this.m_bulletSource && !this.m_bulletSource.IsEnded)
        this.m_bulletSource.ForceStop();
      if (this.setTransformAnimAsBaseState && this.m_state == TransformBehavior.TransformState.Transformed)
        this.m_aiAnimator.ClearBaseAnim();
      this.m_aiAnimator.EndAnimationIf(this.inAnim);
      this.m_aiAnimator.EndAnimationIf(this.transformedAnim);
      this.m_aiAnimator.EndAnimationIf(this.outAnim);
      if (this.requiresTransparency && (bool) (UnityEngine.Object) this.m_cachedShader)
      {
        this.m_aiActor.sprite.usesOverrideMaterial = false;
        this.m_aiActor.renderer.material.shader = this.m_cachedShader;
        this.m_cachedShader = (Shader) null;
      }
      if (this.shadowSupport == TransformBehavior.ShadowSupport.Fade)
        this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f);
      else if (this.shadowSupport == TransformBehavior.ShadowSupport.Animate)
      {
        tk2dSpriteAnimationClip clipByName = this.m_shadowSprite.spriteAnimator.GetClipByName(this.shadowOutAnim);
        this.m_shadowSprite.spriteAnimator.Play(clipByName, (float) (clipByName.frames.Length - 1), clipByName.fps);
      }
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    public override bool IsOverridable() => !this.Uninterruptible;

    public void AnimationEventTriggered(
      tk2dSpriteAnimator animator,
      tk2dSpriteAnimationClip clip,
      int frame)
    {
      string eventInfo = clip.GetFrame(frame).eventInfo;
      if (this.m_shouldFire && eventInfo == "fire")
      {
        if (this.State == TransformBehavior.TransformState.InTrans)
          this.ShootBulletScript(this.inBulletScript);
        else if (this.State == TransformBehavior.TransformState.Transformed)
          this.ShootBulletScript(this.transformedBulletScript);
        else if (this.State == TransformBehavior.TransformState.OutTrans)
          this.ShootBulletScript(this.outBulletScript);
        this.m_shouldFire = false;
      }
      if (this.m_state != TransformBehavior.TransformState.OutTrans && this.m_state != TransformBehavior.TransformState.InTrans)
        return;
      switch (eventInfo)
      {
        case "collider_on":
          this.m_aiActor.IsGone = false;
          this.m_aiActor.specRigidbody.CollideWithOthers = true;
          break;
        case "collider_off":
          this.m_aiActor.IsGone = true;
          this.m_aiActor.specRigidbody.CollideWithOthers = false;
          break;
      }
    }

    private void ShootBulletScript(BulletScriptSelector script)
    {
      if (!(bool) (UnityEngine.Object) this.m_bulletSource)
        this.m_bulletSource = this.shootPoint.GetOrAddComponent<BulletScriptSource>();
      this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
      this.m_bulletSource.BulletScript = script;
      this.m_bulletSource.Initialize();
    }

    private TransformBehavior.TransformState State
    {
      get => this.m_state;
      set
      {
        this.EndState(this.m_state);
        this.m_state = value;
        this.BeginState(this.m_state);
      }
    }

    private void BeginState(TransformBehavior.TransformState state)
    {
      this.m_hasTransitioned = false;
      switch (state)
      {
        case TransformBehavior.TransformState.InTrans:
          if (this.inBulletScript != null && !this.inBulletScript.IsNull)
            this.m_shouldFire = true;
          if (this.requiresTransparency)
          {
            this.m_cachedShader = this.m_aiActor.renderer.material.shader;
            this.m_aiActor.sprite.usesOverrideMaterial = true;
            this.m_aiActor.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
          }
          this.m_aiAnimator.PlayUntilCancelled(this.inAnim, true);
          if (this.shadowSupport == TransformBehavior.ShadowSupport.Animate)
            this.m_shadowSprite.spriteAnimator.PlayAndForceTime(this.shadowInAnim, this.m_aiAnimator.CurrentClipLength);
          this.m_aiActor.ClearPath();
          if (!(bool) (UnityEngine.Object) this.m_aiShooter)
            break;
          this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (TransformBehavior));
          break;
        case TransformBehavior.TransformState.Transformed:
          this.m_timer = this.transformedTime;
          if (this.transformedBulletScript != null && !this.transformedBulletScript.IsNull)
            this.m_shouldFire = true;
          if (!string.IsNullOrEmpty(this.transformedAnim))
            this.m_aiAnimator.PlayUntilCancelled(this.transformedAnim);
          if (this.setTransformAnimAsBaseState)
            this.m_aiAnimator.SetBaseAnim(this.transformedAnim);
          if (this.goneWhileTransformed)
          {
            this.m_aiActor.IsGone = true;
            this.m_aiActor.specRigidbody.CollideWithOthers = false;
          }
          if (!this.transformFireImmediately || this.transformedBulletScript == null || this.transformedBulletScript.IsNull)
            break;
          this.ShootBulletScript(this.transformedBulletScript);
          this.m_shouldFire = false;
          break;
        case TransformBehavior.TransformState.OutTrans:
          if (this.outBulletScript != null && !this.outBulletScript.IsNull)
            this.m_shouldFire = true;
          this.m_aiAnimator.PlayUntilFinished(this.outAnim, true);
          if (this.shadowSupport == TransformBehavior.ShadowSupport.Animate)
            this.m_shadowSprite.spriteAnimator.PlayAndForceTime(this.shadowOutAnim, this.m_aiAnimator.CurrentClipLength);
          if (!(bool) (UnityEngine.Object) this.m_aiShooter)
            break;
          this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (TransformBehavior));
          break;
      }
    }

    private void EndState(TransformBehavior.TransformState state)
    {
      switch (state)
      {
        case TransformBehavior.TransformState.InTrans:
          if (this.inBulletScript != null && !this.inBulletScript.IsNull && this.m_shouldFire)
          {
            this.ShootBulletScript(this.inBulletScript);
            this.m_shouldFire = false;
          }
          if (this.invulnerabilityMode == TransformBehavior.Invulnerability.WhileTransformed)
          {
            this.Invulnerable = true;
            break;
          }
          if (this.invulnerabilityMode != TransformBehavior.Invulnerability.WhileNotTransformed)
            break;
          this.Invulnerable = false;
          break;
        case TransformBehavior.TransformState.Transformed:
          if (this.setTransformAnimAsBaseState)
            this.m_aiAnimator.ClearBaseAnim();
          if (!string.IsNullOrEmpty(this.transformedAnim))
            this.m_aiAnimator.EndAnimationIf(this.transformedAnim);
          if (this.transformedBulletScript == null || this.transformedBulletScript.IsNull || !this.m_shouldFire)
            break;
          this.ShootBulletScript(this.transformedBulletScript);
          this.m_shouldFire = false;
          break;
        case TransformBehavior.TransformState.OutTrans:
          if (this.requiresTransparency && (bool) (UnityEngine.Object) this.m_cachedShader)
          {
            this.m_aiActor.sprite.usesOverrideMaterial = false;
            this.m_aiActor.renderer.material.shader = this.m_cachedShader;
            this.m_cachedShader = (Shader) null;
          }
          if (this.shadowSupport == TransformBehavior.ShadowSupport.Fade)
            this.m_shadowSprite.color = this.m_shadowSprite.color.WithAlpha(1f);
          if (this.goneWhileTransformed)
          {
            this.m_aiActor.IsGone = false;
            this.m_aiActor.specRigidbody.CollideWithOthers = true;
          }
          if ((bool) (UnityEngine.Object) this.m_aiShooter)
            this.m_aiShooter.ToggleGunAndHandRenderers(true, nameof (TransformBehavior));
          if (this.outBulletScript != null && !this.outBulletScript.IsNull && this.m_shouldFire)
          {
            this.ShootBulletScript(this.outBulletScript);
            this.m_shouldFire = false;
          }
          if (this.invulnerabilityMode == TransformBehavior.Invulnerability.WhileTransformed)
          {
            this.Invulnerable = false;
            break;
          }
          if (this.invulnerabilityMode != TransformBehavior.Invulnerability.WhileNotTransformed)
            break;
          this.Invulnerable = true;
          break;
      }
    }

    private bool Invulnerable
    {
      get => this.m_isInvulnerable;
      set
      {
        if (value == this.m_isInvulnerable)
          return;
        this.m_enemyHitbox.Enabled = !value;
        this.m_aiActor.healthHaver.IsVulnerable = !value;
        if (this.m_bulletBlocker != null)
          this.m_bulletBlocker.Enabled = value;
        if (this.reflectBullets && this.m_aiActor.healthHaver.IsAlive)
        {
          this.m_aiActor.specRigidbody.ReflectProjectiles = value;
          this.m_aiActor.specRigidbody.ReflectBeams = value;
          if (value)
          {
            this.m_aiActor.specRigidbody.ReflectProjectilesNormalGenerator = new Func<Vector2, Vector2, Vector2>(this.GetNormal);
            this.m_aiActor.specRigidbody.ReflectBeamsNormalGenerator = new Func<Vector2, Vector2, Vector2>(this.GetNormal);
          }
        }
        this.m_isInvulnerable = value;
      }
    }

    private Vector2 GetNormal(Vector2 contact, Vector2 normal)
    {
      return (contact - this.m_bulletBlocker.UnitCenter).normalized;
    }

    public enum ShadowSupport
    {
      None = 10, // 0x0000000A
      Fade = 20, // 0x00000014
      Animate = 30, // 0x0000001E
    }

    public enum Invulnerability
    {
      None = 10, // 0x0000000A
      WhileTransformed = 20, // 0x00000014
      WhileNotTransformed = 30, // 0x0000001E
    }

    private enum TransformState
    {
      None,
      InTrans,
      Transformed,
      OutTrans,
    }
  }

