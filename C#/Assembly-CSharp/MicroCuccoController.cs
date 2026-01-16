// Decompiled with JetBrains decompiler
// Type: MicroCuccoController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class MicroCuccoController : BraveBehaviour
{
  public float Speed = 8f;
  public float Damage = 4f;
  private PlayerController m_owner;
  private AIActor m_target;

  public void Initialize(PlayerController owner)
  {
    this.m_owner = owner;
    this.StartCoroutine(this.FindTarget());
  }

  private void Update()
  {
    if (!(bool) (Object) this.m_target)
      return;
    Vector2 vector2 = this.m_target.CenterPosition - this.sprite.WorldCenter;
    if ((double) vector2.x > 0.0)
    {
      if (!this.spriteAnimator.IsPlaying("attack_right"))
        this.spriteAnimator.Play("attack_right");
    }
    else if (!this.spriteAnimator.IsPlaying("attack_left"))
      this.spriteAnimator.Play("attack_left");
    if ((double) vector2.magnitude < 0.5)
    {
      float num = 1f;
      if (PassiveItem.IsFlagSetAtAll(typeof (BattleStandardItem)) && (bool) (Object) this.m_owner && (bool) (Object) this.m_owner.CurrentGun && this.m_owner.CurrentGun.IsLuteCompanionBuff)
        num = BattleStandardItem.BattleStandardCompanionDamageMultiplier;
      this.m_target.healthHaver.ApplyDamage(num * this.Damage, Vector2.zero, "Cucco");
      Object.Destroy((Object) this.gameObject);
    }
    else
      this.transform.position = this.transform.position + (vector2.normalized * this.Speed * BraveTime.DeltaTime).ToVector3ZUp();
  }

  [DebuggerHidden]
  private IEnumerator FindTarget()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new MicroCuccoController.\u003CFindTarget\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
