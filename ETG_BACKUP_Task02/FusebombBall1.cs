// Decompiled with JetBrains decompiler
// Type: FusebombBall1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/Fusebomb/Ball1")]
public class FusebombBall1 : Script
{
  protected override IEnumerator Top()
  {
    Random.Range(-30f, 30f);
    this.Fire(new Brave.BulletScript.Direction(), new Brave.BulletScript.Speed(3f), (Bullet) new FusebombBall1.RollyBall());
    return (IEnumerator) null;
  }

  private class RollyBall : Bullet
  {
    private const float TargetSpeed = 12f;

    public RollyBall()
      : base("ball")
    {
    }

    protected override IEnumerator Top()
    {
      float direction = (float) -Random.Range(20, 55);
      this.ChangeSpeed(new Brave.BulletScript.Speed(12f), 60);
      this.ChangeDirection(new Brave.BulletScript.Direction(direction), 60);
      return (IEnumerator) null;
    }

    public override void OnForceRemoved()
    {
      this.Speed = 12f;
      if (!(bool) (Object) this.Projectile || !(bool) (Object) this.Projectile.specRigidbody || !(this.Projectile.specRigidbody.Velocity != Vector2.zero))
        return;
      this.Projectile.specRigidbody.Velocity = this.Projectile.specRigidbody.Velocity.normalized * 12f;
    }
  }
}
