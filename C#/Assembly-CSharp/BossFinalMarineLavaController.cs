// Decompiled with JetBrains decompiler
// Type: BossFinalMarineLavaController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BossFinalMarineLavaController : BraveBehaviour
{
  public GoopDefinition goopDefinition;
  private DimensionFogController m_dimensionFog;

  public void Start()
  {
    this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnTriggerCollision);
    this.m_dimensionFog = Object.FindObjectOfType<DimensionFogController>();
  }

  private void OnTriggerCollision(
    SpeculativeRigidbody speculativeRigidbody,
    SpeculativeRigidbody sourceSpecRigidbody,
    CollisionData collisionData)
  {
    PlayerController component = speculativeRigidbody.GetComponent<PlayerController>();
    Vector2 unitCenter = component.specRigidbody.GetUnitCenter(ColliderType.HitBox);
    if (!component.spriteAnimator.QueryGroundedFrame() || (double) Vector2.Distance(unitCenter, (Vector2) this.m_dimensionFog.transform.position) >= (double) this.m_dimensionFog.ApparentRadius)
      return;
    component.IncreasePoison(BraveTime.DeltaTime * 1.5f);
    if ((double) component.CurrentPoisonMeterValue < 1.0)
      return;
    component.healthHaver.ApplyDamage(0.5f, Vector2.zero, StringTableManager.GetEnemiesString("#GOOP"), CoreDamageTypes.Poison, DamageCategory.Environment, true);
    component.CurrentPoisonMeterValue = 0.0f;
  }
}
