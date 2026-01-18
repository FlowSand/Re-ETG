// Decompiled with JetBrains decompiler
// Type: DraGunThrowKnifeBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/ThrowKnifeBehavior")]
public class DraGunThrowKnifeBehavior : BasicAttackBehavior
  {
    public float delay;
    public GameObject ShootPoint;
    public string BulletName;
    public float angle;
    public Animation unityAnimation;
    public string unityShootAnim;
    public AIAnimator aiAnimator;
    public string aiShootAnim;
    private DraGunController m_dragun;
    private float m_timer;
    private bool m_isAttacking;

    public override void Start()
    {
      base.Start();
      this.m_dragun = this.m_aiActor.GetComponent<DraGunController>();
      if (!(bool) (UnityEngine.Object) this.aiAnimator)
        return;
      this.aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_timer);
    }

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      if ((double) this.delay <= 0.0)
      {
        this.StartThrow();
      }
      else
      {
        this.m_timer = this.delay;
        this.m_isAttacking = false;
      }
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      if (!this.m_isAttacking)
      {
        if ((double) this.m_timer <= 0.0)
          this.StartThrow();
      }
      else
      {
        bool flag = true;
        if ((bool) (UnityEngine.Object) this.unityAnimation)
          flag &= !this.unityAnimation.IsPlaying(this.unityShootAnim);
        if ((bool) (UnityEngine.Object) this.aiAnimator)
          flag &= !this.aiAnimator.IsPlaying(this.aiShootAnim);
        if (flag)
          return ContinuousBehaviorResult.Finished;
      }
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      if ((bool) (UnityEngine.Object) this.aiAnimator)
        this.aiAnimator.EndAnimation();
      if ((bool) (UnityEngine.Object) this.unityAnimation)
      {
        this.unityAnimation.Stop();
        this.unityAnimation.GetClip(this.unityShootAnim).SampleAnimation(this.unityAnimation.gameObject, 1000f);
        this.unityAnimation.GetComponent<DraGunArmController>().UnclipHandSprite();
      }
      this.m_isAttacking = false;
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    public override bool IsOverridable() => false;

    public override bool IsReady()
    {
      if (!base.IsReady())
        return false;
      List<AIActor> activeEnemies = this.m_aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
      for (int index = 0; index < activeEnemies.Count; ++index)
      {
        if (activeEnemies[index].name.Contains("knife", true))
          return false;
      }
      return true;
    }

    private void AnimationEventTriggered(
      tk2dSpriteAnimator spriteAnimator,
      tk2dSpriteAnimationClip clip,
      int frame)
    {
      if (!this.m_isAttacking || !(clip.GetFrame(frame).eventInfo == "fire"))
        return;
      this.m_dragun.bulletBank.CreateProjectileFromBank((Vector2) this.ShootPoint.transform.position, this.angle, "knife");
    }

    private void StartThrow()
    {
      if ((bool) (UnityEngine.Object) this.unityAnimation)
        this.unityAnimation.Play(this.unityShootAnim);
      if ((bool) (UnityEngine.Object) this.aiAnimator)
        this.aiAnimator.PlayUntilCancelled(this.aiShootAnim);
      this.m_isAttacking = true;
    }
  }

