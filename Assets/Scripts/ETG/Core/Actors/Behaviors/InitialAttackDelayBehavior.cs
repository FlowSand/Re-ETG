// Decompiled with JetBrains decompiler
// Type: InitialAttackDelayBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class InitialAttackDelayBehavior : AttackBehaviorBase
  {
    public float Time = 2f;
    public string PlayDirectionalAnimation;
    public string SetDefaultDirectionalAnimation;
    public bool EndOnDamage;
    private float m_timer;
    private bool m_done;

    public override void Start()
    {
      base.Start();
      if (!(bool) (Object) this.m_aiActor.healthHaver || !this.EndOnDamage)
        return;
      this.m_aiActor.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnDamaged);
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_timer);
    }

    public override BehaviorResult Update()
    {
      int num = (int) base.Update();
      if (this.m_done)
        return BehaviorResult.Continue;
      if (!string.IsNullOrEmpty(this.PlayDirectionalAnimation))
        this.m_aiAnimator.PlayUntilFinished(this.PlayDirectionalAnimation, true);
      if (!string.IsNullOrEmpty(this.SetDefaultDirectionalAnimation))
        this.m_aiAnimator.SetBaseAnim(this.SetDefaultDirectionalAnimation);
      this.m_timer = this.Time;
      return BehaviorResult.RunContinuousInClass;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      if ((double) this.m_timer <= 0.0)
        return ContinuousBehaviorResult.Finished;
      this.m_aiActor.ClearPath();
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      if (!string.IsNullOrEmpty(this.PlayDirectionalAnimation))
        this.m_aiAnimator.EndAnimationIf(this.PlayDirectionalAnimation);
      this.m_done = true;
      if (!(bool) (Object) this.m_aiActor.healthHaver || !this.EndOnDamage)
        return;
      this.m_aiActor.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnDamaged);
    }

    public override bool IsReady() => !this.m_done;

    public override float GetMinReadyRange() => -1f;

    public override float GetMaxRange() => -1f;

    private void OnDamaged(
      float resultValue,
      float maxValue,
      CoreDamageTypes damageTypes,
      DamageCategory damageCategory,
      Vector2 damageDirection)
    {
      if (!this.EndOnDamage)
        return;
      this.m_timer = 0.0f;
    }
  }

