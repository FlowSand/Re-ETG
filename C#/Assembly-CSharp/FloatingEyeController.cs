// Decompiled with JetBrains decompiler
// Type: FloatingEyeController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable
public class FloatingEyeController : BraveBehaviour
{
  private BeholsterController m_beholster;
  private bool m_beholsterKilled;

  public void Awake()
  {
    if ((bool) (UnityEngine.Object) this.aiAnimator)
      this.aiAnimator.OnSpawnCompleted += new System.Action(this.OnSpawnCompleted);
    this.aiActor.PreventAutoKillOnBossDeath = true;
  }

  public void Start()
  {
    this.m_beholster = UnityEngine.Object.FindObjectOfType<BeholsterController>();
    if (!(bool) (UnityEngine.Object) this.m_beholster)
      return;
    this.m_beholster.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnBeholsterDamaged);
  }

  protected override void OnDestroy()
  {
    if ((bool) (UnityEngine.Object) this.m_beholster)
      this.m_beholster.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnBeholsterDamaged);
    if ((bool) (UnityEngine.Object) this.aiAnimator)
      this.aiAnimator.OnSpawnCompleted -= new System.Action(this.OnSpawnCompleted);
    base.OnDestroy();
  }

  private void OnBeholsterDamaged(
    float resultValue,
    float maxValue,
    CoreDamageTypes damageTypes,
    DamageCategory damageCategory,
    Vector2 damageDirection)
  {
    if ((double) resultValue > 0.0)
      return;
    this.m_beholsterKilled = true;
    this.StartCrying();
  }

  private void OnSpawnCompleted()
  {
    if ((bool) (UnityEngine.Object) this.aiActor)
      this.aiActor.PathableTiles |= CellTypes.PIT;
    if (!this.m_beholsterKilled && (!(bool) (UnityEngine.Object) this.m_beholster || !this.m_beholster.healthHaver.IsDead))
      return;
    this.StartCrying();
  }

  private void StartCrying()
  {
    this.aiActor.ClearPath();
    this.behaviorSpeculator.enabled = false;
    this.aiShooter.enabled = false;
    this.aiShooter.ToggleGunAndHandRenderers(false, "Cry");
    this.aiActor.IgnoreForRoomClear = true;
    this.aiAnimator.PlayUntilCancelled("cry");
  }
}
