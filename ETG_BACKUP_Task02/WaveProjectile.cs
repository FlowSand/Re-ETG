// Decompiled with JetBrains decompiler
// Type: WaveProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class WaveProjectile : Projectile
{
  public float amplitude = 1f;
  public float frequency = 2f;

  protected override void Move()
  {
    this.m_timeElapsed += this.LocalDeltaTime;
    this.specRigidbody.Velocity = (Vector2) (this.transform.right * this.baseData.speed + this.transform.up * ((float) ((!this.Inverted ? 1.0 : -1.0) * (double) this.amplitude * 2.0 * 3.1415927410125732) * this.frequency * Mathf.Cos((float) ((double) this.m_timeElapsed * 2.0 * 3.1415927410125732) * this.frequency)));
  }

  protected override void OnDestroy() => base.OnDestroy();
}
