// Decompiled with JetBrains decompiler
// Type: MetalGearRatController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;

#nullable disable
public class MetalGearRatController : BraveBehaviour
{
  public MetalGearRatController.MetalGearPart Tailgun;
  public MetalGearRatController.MetalGearPart Radome;
  public float MinBodyDamageHealth = 0.15f;
  public string IconAttackName = "Icon Stomp";
  public float IconAttackProbability = 4f;
  private PixelCollider m_tailgunPixelCollider;
  private bool m_isTailgunDestroyed;
  private PixelCollider m_radomePixelCollider;
  private bool m_isRadomeDestroyed;

  public void Start()
  {
    this.m_tailgunPixelCollider = this.specRigidbody.PixelColliders[this.Tailgun.PixelCollider];
    this.m_radomePixelCollider = this.specRigidbody.PixelColliders[this.Radome.PixelCollider];
    this.healthHaver.AddTrackedDamagePixelCollider(this.m_tailgunPixelCollider);
    this.healthHaver.AddTrackedDamagePixelCollider(this.m_radomePixelCollider);
    this.healthHaver.GlobalPixelColliderDamageMultiplier = 0.25f;
  }

  public void Update()
  {
    if (!this.m_isTailgunDestroyed)
    {
      float num1 = 1f;
      float num2;
      if (this.healthHaver.PixelColliderDamage.TryGetValue(this.m_tailgunPixelCollider, out num2))
      {
        float num3 = num2 / this.healthHaver.GetMaxHealth();
        num1 = (float) (1.0 - (double) num3 / (double) this.Tailgun.HealthPercentage);
        if ((double) num3 >= (double) this.Tailgun.HealthPercentage && (bool) (UnityEngine.Object) this.behaviorSpeculator && this.behaviorSpeculator.IsInterruptable)
        {
          this.m_isTailgunDestroyed = true;
          this.StartCoroutine(this.DestroyPartCR(this.Tailgun, "destroy_tailgun"));
          num1 = 0.0f;
        }
      }
    }
    if (this.m_isRadomeDestroyed)
      return;
    float num4 = 1f;
    float num5;
    if (!this.healthHaver.PixelColliderDamage.TryGetValue(this.m_radomePixelCollider, out num5))
      return;
    float num6 = num5 / this.healthHaver.GetMaxHealth();
    num4 = (float) (1.0 - (double) num6 / (double) this.Radome.HealthPercentage);
    if ((double) num6 <= (double) this.Radome.HealthPercentage || !(bool) (UnityEngine.Object) this.behaviorSpeculator || !this.behaviorSpeculator.IsInterruptable)
      return;
    this.m_isRadomeDestroyed = true;
    this.StartCoroutine(this.DestroyPartCR(this.Radome, "destroy_radome"));
    num4 = 0.0f;
  }

  [DebuggerHidden]
  private IEnumerator DestroyPartCR(MetalGearRatController.MetalGearPart part, string destroyAnim)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new MetalGearRatController.\u003CDestroyPartCR\u003Ec__Iterator0()
    {
      destroyAnim = destroyAnim,
      part = part,
      \u0024this = this
    };
  }

  [Serializable]
  public class MetalGearPart
  {
    public AIAnimator AIAnimator;
    public float HealthPercentage = 0.1f;
    public int PixelCollider;
    public int DeathPixelCollider;
    public string AttackName;
    public AutoAimTarget AutoAimer;
    public float BodyDamageOnDeath;
  }
}
