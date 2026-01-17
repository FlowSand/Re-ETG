// Decompiled with JetBrains decompiler
// Type: CollisionTriggerEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CollisionTriggerEvent : BraveBehaviour
{
  public bool onTriggerEnter;
  public bool onTriggerCollision;
  public bool onTriggerExit;
  public float delay;
  public string animationName;
  public bool destroyAfterAnimation;
  public VFXPool vfx;
  public Vector2 vfxOffset;
  private bool m_triggered;
  private float m_timer;

  public void Start()
  {
    if (this.onTriggerEnter)
      this.specRigidbody.OnEnterTrigger += new SpeculativeRigidbody.OnTriggerDelegate(this.OnTrigger);
    if (this.onTriggerCollision)
      this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnTrigger);
    if (!this.onTriggerExit)
      return;
    this.specRigidbody.OnExitTrigger += new SpeculativeRigidbody.OnTriggerExitDelegate(this.OnTriggerSimple);
  }

  public void Update()
  {
    if (!this.m_triggered)
      return;
    this.m_timer -= BraveTime.DeltaTime;
    if ((double) this.m_timer > 0.0)
      return;
    this.DoEventStuff();
  }

  private void OnTrigger(
    SpeculativeRigidbody specRigidbody,
    SpeculativeRigidbody sourceSpecRigidbody,
    CollisionData collisionData)
  {
    this.OnTriggerSimple(specRigidbody, sourceSpecRigidbody);
  }

  private void OnTriggerSimple(
    SpeculativeRigidbody specRigidbody,
    SpeculativeRigidbody sourceSpecRigidbody)
  {
    if ((double) this.delay <= 0.0)
    {
      this.DoEventStuff();
    }
    else
    {
      this.m_triggered = true;
      this.m_timer = this.delay;
    }
  }

  private void DoEventStuff()
  {
    if (!string.IsNullOrEmpty(this.animationName) && (bool) (Object) this.spriteAnimator)
    {
      this.spriteAnimator.Play(this.animationName);
      if (this.destroyAfterAnimation)
        this.gameObject.AddComponent<SpriteAnimatorKiller>();
    }
    this.vfx.SpawnAtLocalPosition((Vector3) this.vfxOffset, 0.0f, this.transform, new Vector2?(Vector2.zero), new Vector2?(Vector2.zero));
    Object.Destroy((Object) this);
  }

  protected override void OnDestroy() => base.OnDestroy();
}
