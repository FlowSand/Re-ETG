// Decompiled with JetBrains decompiler
// Type: ReflectShieldPlayerItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class ReflectShieldPlayerItem : PlayerItem
{
  public float duration = 5f;
  protected SpeculativeRigidbody userSRB;
  private bool m_usedOverrideMaterial;

  protected override void DoEffect(PlayerController user)
  {
    this.userSRB = user.specRigidbody;
    user.StartCoroutine(this.HandleShield(user));
    int num = (int) AkSoundEngine.PostEvent("Play_OBJ_metalskin_activate_01", this.gameObject);
  }

  [DebuggerHidden]
  private IEnumerator HandleShield(PlayerController user)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ReflectShieldPlayerItem.\u003CHandleShield\u003Ec__Iterator0()
    {
      user = user,
      \u0024this = this
    };
  }

  private void OnPreCollision(
    SpeculativeRigidbody myRigidbody,
    PixelCollider myCollider,
    SpeculativeRigidbody otherRigidbody,
    PixelCollider otherCollider)
  {
    Projectile component = otherRigidbody.GetComponent<Projectile>();
    if (!((Object) component != (Object) null) || component.Owner is PlayerController)
      return;
    PassiveReflectItem.ReflectBullet(component, true, this.userSRB.gameActor, 10f);
    PhysicsEngine.SkipCollision = true;
  }

  protected override void OnPreDrop(PlayerController user)
  {
    if (!this.IsCurrentlyActive)
      return;
    this.StopAllCoroutines();
    if ((bool) (Object) user)
    {
      user.healthHaver.IsVulnerable = true;
      user.ClearOverrideShader();
      user.sprite.usesOverrideMaterial = this.m_usedOverrideMaterial;
      user.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision);
      this.IsCurrentlyActive = false;
    }
    if (!(bool) (Object) this)
      return;
    int num = (int) AkSoundEngine.PostEvent("Play_OBJ_metalskin_end_01", this.gameObject);
  }

  protected override void OnDestroy() => base.OnDestroy();
}
