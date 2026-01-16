// Decompiled with JetBrains decompiler
// Type: BehaviorBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[Serializable]
public abstract class BehaviorBase
{
  protected GameObject m_gameObject;
  protected AIActor m_aiActor;
  protected AIShooter m_aiShooter;
  protected AIAnimator m_aiAnimator;
  protected float m_deltaTime;
  protected bool m_updateEveryFrame;
  protected bool m_ignoreGlobalCooldown;

  public virtual void Start()
  {
  }

  public virtual void Upkeep()
  {
  }

  public virtual bool OverrideOtherBehaviors() => false;

  public virtual BehaviorResult Update() => BehaviorResult.Continue;

  public virtual ContinuousBehaviorResult ContinuousUpdate() => ContinuousBehaviorResult.Continue;

  public virtual void EndContinuousUpdate()
  {
  }

  public virtual void OnActorPreDeath()
  {
  }

  public virtual void Destroy()
  {
  }

  public virtual void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
  {
    this.m_gameObject = gameObject;
    this.m_aiActor = aiActor;
    this.m_aiShooter = aiShooter;
    this.m_aiAnimator = gameObject.GetComponent<AIAnimator>();
  }

  public virtual void SetDeltaTime(float deltaTime) => this.m_deltaTime = deltaTime;

  public virtual bool UpdateEveryFrame() => this.m_updateEveryFrame;

  public virtual bool IgnoreGlobalCooldown() => this.m_ignoreGlobalCooldown;

  public virtual bool IsOverridable() => true;

  protected void DecrementTimer(ref float timer, bool useCooldownFactor = false)
  {
    float deltaTime = this.m_deltaTime;
    if ((bool) (UnityEngine.Object) this.m_aiActor && useCooldownFactor)
      deltaTime *= this.m_aiActor.behaviorSpeculator.CooldownScale;
    timer = Mathf.Max(timer - deltaTime, 0.0f);
  }
}
