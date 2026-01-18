// Decompiled with JetBrains decompiler
// Type: WaitForAnimationComplete
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class WaitForAnimationComplete : OverrideBehaviorBase
  {
    public string[] TargetAnimations;
    public float ExtraDelay;
    protected float remainingDelay;

    public override void Start()
    {
      base.Start();
      this.remainingDelay = this.ExtraDelay;
    }

    public override void Upkeep() => base.Upkeep();

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      for (int index = 0; index < this.TargetAnimations.Length; ++index)
      {
        if ((Object) this.m_aiAnimator != (Object) null)
        {
          if (this.m_aiAnimator.IsPlaying(this.TargetAnimations[index]))
          {
            this.remainingDelay = this.ExtraDelay;
            return BehaviorResult.SkipAllRemainingBehaviors;
          }
        }
        else if (this.m_aiActor.spriteAnimator.IsPlaying(this.TargetAnimations[index]))
        {
          this.remainingDelay = this.ExtraDelay;
          return BehaviorResult.SkipAllRemainingBehaviors;
        }
      }
      if ((double) this.remainingDelay <= 0.0)
        return behaviorResult;
      this.remainingDelay -= this.m_deltaTime;
      return BehaviorResult.SkipAllRemainingBehaviors;
    }
  }

