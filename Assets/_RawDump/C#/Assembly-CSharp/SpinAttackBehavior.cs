// Decompiled with JetBrains decompiler
// Type: SpinAttackBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using UnityEngine;

#nullable disable
public class SpinAttackBehavior : BasicAttackBehavior
{
  public static bool ConcurrentAttackHappening;
  public GameObject ShootPoint;
  public BulletScriptSelector BulletScript;
  public bool PreventConcurrentAttacks;
  [InspectorCategory("Visuals")]
  public string spinBeginAnim = "spin_begin";
  [InspectorCategory("Visuals")]
  public string spinAnim = "spin";
  [InspectorCategory("Visuals")]
  public string spinEndAnim = "spin_end";
  [InspectorCategory("Visuals")]
  public bool SpawnMuzzleVfx;
  [InspectorShowIf("SpawnMuzzleVfx")]
  [InspectorCategory("Visuals")]
  public float VfxMidDelay = 0.1f;
  [InspectorCategory("Visuals")]
  public string spinVfx;
  private SpinAttackBehavior.SpinState m_state;
  private float m_vfxTimer;
  private BulletScriptSource m_bulletSource;

  public override void Start()
  {
    base.Start();
    if (!this.PreventConcurrentAttacks)
      return;
    SpinAttackBehavior.ConcurrentAttackHappening = false;
  }

  public override BehaviorResult Update()
  {
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (!this.IsReady())
      return BehaviorResult.Continue;
    this.m_state = SpinAttackBehavior.SpinState.Start;
    this.m_aiAnimator.PlayUntilFinished(this.spinBeginAnim, true);
    if (this.PreventConcurrentAttacks)
      SpinAttackBehavior.ConcurrentAttackHappening = true;
    this.m_vfxTimer = this.VfxMidDelay;
    this.m_aiActor.ClearPath();
    this.m_updateEveryFrame = true;
    return BehaviorResult.RunContinuous;
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    int num = (int) base.ContinuousUpdate();
    if (this.m_state == SpinAttackBehavior.SpinState.Start)
    {
      if (!this.m_aiAnimator.IsPlaying(this.spinBeginAnim))
      {
        this.m_state = SpinAttackBehavior.SpinState.Spin;
        this.m_aiAnimator.PlayUntilFinished(this.spinAnim, true);
        if (!string.IsNullOrEmpty(this.spinVfx))
          this.m_aiAnimator.PlayVfx(this.spinVfx);
        this.ShootBulletScript();
      }
    }
    else if (this.m_state == SpinAttackBehavior.SpinState.Spin)
    {
      if (this.SpawnMuzzleVfx)
      {
        this.m_vfxTimer -= this.m_deltaTime;
        if ((double) this.VfxMidDelay > 0.0 && (double) this.m_vfxTimer <= 0.0)
        {
          VFXPool muzzleFlashEffects = this.m_aiActor.bulletBank.GetBullet().MuzzleFlashEffects;
          PixelCollider hitboxPixelCollider = this.m_aiActor.specRigidbody.HitboxPixelCollider;
          Vector2 unitBottomLeft = hitboxPixelCollider.UnitBottomLeft;
          Vector2 unitCenter = hitboxPixelCollider.UnitCenter;
          Vector2 unitTopRight = hitboxPixelCollider.UnitTopRight;
          for (; (double) this.m_vfxTimer <= 0.0; this.m_vfxTimer += this.VfxMidDelay)
          {
            Vector2 vector2 = BraveUtility.RandomVector2(unitBottomLeft, unitTopRight, new Vector2(-1.5f, -1.5f));
            float angle = (vector2 - unitCenter).ToAngle();
            muzzleFlashEffects.SpawnAtLocalPosition((Vector3) (vector2 - this.ShootPoint.transform.position.XY()), angle, this.ShootPoint.transform);
          }
        }
      }
      if (this.m_bulletSource.IsEnded)
      {
        this.m_state = SpinAttackBehavior.SpinState.End;
        if (!string.IsNullOrEmpty(this.spinEndAnim))
          this.m_aiAnimator.PlayUntilFinished(this.spinEndAnim, true);
        else
          this.m_aiAnimator.EndAnimationIf(this.spinAnim);
        if (!string.IsNullOrEmpty(this.spinVfx))
          this.m_aiAnimator.StopVfx(this.spinVfx);
      }
    }
    else if (this.m_state == SpinAttackBehavior.SpinState.End && !this.m_aiAnimator.IsPlaying(this.spinEndAnim))
    {
      this.m_state = SpinAttackBehavior.SpinState.None;
      return ContinuousBehaviorResult.Finished;
    }
    return ContinuousBehaviorResult.Continue;
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    this.m_state = SpinAttackBehavior.SpinState.None;
    if ((bool) (Object) this.m_bulletSource && !this.m_bulletSource.IsEnded)
      this.m_bulletSource.ForceStop();
    this.m_aiAnimator.EndAnimationIf(this.spinBeginAnim);
    this.m_aiAnimator.EndAnimationIf(this.spinAnim);
    this.m_aiAnimator.EndAnimationIf(this.spinEndAnim);
    this.m_aiAnimator.StopVfx(this.spinVfx);
    if (this.PreventConcurrentAttacks)
      SpinAttackBehavior.ConcurrentAttackHappening = false;
    this.m_updateEveryFrame = false;
    this.UpdateCooldowns();
  }

  public override void OnActorPreDeath()
  {
    base.OnActorPreDeath();
    if (!this.PreventConcurrentAttacks)
      return;
    SpinAttackBehavior.ConcurrentAttackHappening = false;
  }

  public override bool IsReady()
  {
    return (!this.PreventConcurrentAttacks || !SpinAttackBehavior.ConcurrentAttackHappening) && base.IsReady();
  }

  private void ShootBulletScript()
  {
    if (!(bool) (Object) this.m_bulletSource)
      this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
    this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
    this.m_bulletSource.BulletScript = this.BulletScript;
    this.m_bulletSource.Initialize();
  }

  private enum SpinState
  {
    None,
    Start,
    Spin,
    End,
  }
}
