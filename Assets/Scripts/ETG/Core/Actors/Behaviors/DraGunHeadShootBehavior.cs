using FullInspector;
using System;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/HeadShootBehavior")]
public class DraGunHeadShootBehavior : BasicAttackBehavior
  {
    public GameObject ShootPoint;
    public BulletScriptSelector BulletScript;
    public DraGunHeadShootBehavior.FireType fireType;
    public string HeadAiAnim = "sweep";
    public string MotionClip = "DraGunHeadSweep1";
    private DraGunController m_dragun;
    private DraGunHeadController m_head;
    private AIAnimator m_headAnimator;
    private Animation m_unityAnimation;
    private DraGunHeadShootBehavior.State m_state;
    private AnimationClip m_clip;
    private float m_cachedShootPointRotation;
    private BulletScriptSource m_bulletSource;
    private GameObject s_dummyGameObject;
    private GameObject s_dummyHeadObject;

    public override void Start()
    {
      base.Start();
      this.m_dragun = this.m_aiActor.GetComponent<DraGunController>();
      this.m_unityAnimation = this.m_dragun.neck.GetComponent<Animation>();
      this.m_head = this.m_dragun.head;
      this.m_headAnimator = this.m_head.aiAnimator;
      this.m_clip = this.m_unityAnimation.GetClip(this.MotionClip);
      if (this.fireType == DraGunHeadShootBehavior.FireType.tk2dAnimEvent)
        this.m_head.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.tk2dAnimationEventTriggered);
      if (this.fireType != DraGunHeadShootBehavior.FireType.UnityAnimEvent)
        return;
      this.m_aiActor.behaviorSpeculator.AnimationEventTriggered += new Action<string>(this.UnityAnimationEventTriggered);
    }

    public override void Upkeep() => base.Upkeep();

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      this.m_state = DraGunHeadShootBehavior.State.MovingToPosition;
      this.m_head.OverrideDesiredPosition = new Vector2?(this.GetStartPosition());
      if ((bool) (UnityEngine.Object) this.ShootPoint)
        this.m_cachedShootPointRotation = this.ShootPoint.transform.eulerAngles.z;
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      if (this.m_state == DraGunHeadShootBehavior.State.MovingToPosition)
      {
        if (this.m_head.ReachedOverridePosition)
        {
          this.m_state = DraGunHeadShootBehavior.State.Animating;
          this.m_head.OverrideDesiredPosition = new Vector2?();
          this.m_headAnimator.AnimatedFacingDirection = this.m_headAnimator.FacingDirection;
          this.m_headAnimator.UseAnimatedFacingDirection = true;
          if (this.fireType == DraGunHeadShootBehavior.FireType.Immediate)
            this.ShootBulletScript();
          this.m_clip.SampleAnimation(this.m_aiActor.gameObject, 0.0f);
          this.m_unityAnimation.Stop();
          this.m_unityAnimation.cullingType = AnimationCullingType.BasedOnRenderers;
          this.m_unityAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
          this.m_unityAnimation.Play(this.MotionClip);
          this.m_headAnimator.PlayUntilCancelled(this.HeadAiAnim);
        }
      }
      else if (this.m_state == DraGunHeadShootBehavior.State.Animating)
      {
        if ((bool) (UnityEngine.Object) this.ShootPoint)
          this.ShootPoint.transform.rotation = Quaternion.Euler(this.ShootPoint.transform.eulerAngles.WithZ(this.m_headAnimator.FacingDirection));
        if (!this.m_unityAnimation.IsPlaying(this.MotionClip))
          return ContinuousBehaviorResult.Finished;
      }
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      this.m_state = DraGunHeadShootBehavior.State.None;
      this.m_headAnimator.UseAnimatedFacingDirection = false;
      this.m_headAnimator.EndAnimationIf(this.HeadAiAnim);
      if ((bool) (UnityEngine.Object) this.m_unityAnimation)
      {
        this.m_unityAnimation.Stop();
        this.m_unityAnimation.GetClip(this.MotionClip).SampleAnimation(this.m_unityAnimation.gameObject, 1000f);
      }
      if ((bool) (UnityEngine.Object) this.m_bulletSource)
        this.m_bulletSource.ForceStop();
      if ((bool) (UnityEngine.Object) this.ShootPoint)
        this.ShootPoint.transform.rotation = Quaternion.Euler(this.ShootPoint.transform.eulerAngles.WithZ(this.m_cachedShootPointRotation));
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    public override bool IsOverridable() => false;

    private void tk2dAnimationEventTriggered(
      tk2dSpriteAnimator animator,
      tk2dSpriteAnimationClip clip,
      int frame)
    {
      this.UnityAnimationEventTriggered(clip.GetFrame(frame).eventInfo);
    }

    private void UnityAnimationEventTriggered(string eventInfo)
    {
      switch (eventInfo)
      {
        case "fire":
          this.ShootBulletScript();
          break;
        case "cease_fire":
          if (!(bool) (UnityEngine.Object) this.m_bulletSource)
            break;
          this.m_bulletSource.ForceStop();
          break;
      }
    }

    private void ShootBulletScript()
    {
      if (string.IsNullOrEmpty(this.BulletScript.scriptTypeName))
        return;
      if (!(bool) (UnityEngine.Object) this.m_bulletSource)
        this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
      this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
      this.m_bulletSource.BulletScript = this.BulletScript;
      this.m_bulletSource.Initialize();
    }

    private Vector2 GetStartPosition()
    {
      if (!(bool) (UnityEngine.Object) this.s_dummyGameObject)
      {
        this.s_dummyGameObject = new GameObject("Dummy Game Object");
        this.s_dummyHeadObject = new GameObject("head");
        this.s_dummyHeadObject.transform.parent = this.s_dummyGameObject.transform;
      }
      this.m_clip.SampleAnimation(this.s_dummyGameObject, 0.0f);
      return (Vector2) (this.s_dummyHeadObject.transform.position + this.m_aiActor.transform.position);
    }

    private enum State
    {
      None,
      MovingToPosition,
      Animating,
    }

    public enum FireType
    {
      Immediate,
      tk2dAnimEvent,
      UnityAnimEvent,
    }
  }

