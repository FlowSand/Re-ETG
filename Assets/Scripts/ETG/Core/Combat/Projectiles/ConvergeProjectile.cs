// Decompiled with JetBrains decompiler
// Type: ConvergeProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

public class ConvergeProjectile : Projectile
  {
    public float convergeDistance = 10f;
    public float amplitude = 1f;

    protected override void Move()
    {
      this.m_timeElapsed += BraveTime.DeltaTime;
      if ((double) this.m_timeElapsed < (double) (this.convergeDistance / this.baseData.speed))
        this.specRigidbody.Velocity = (Vector2) (this.transform.right * this.baseData.speed + this.transform.up * ((float) ((!this.Inverted ? 1.0 : -1.0) * (double) this.amplitude * 2.0 * 3.1415927410125732 * ((double) this.baseData.speed / ((double) this.convergeDistance * 2.0))) * Mathf.Cos((float) ((double) this.m_timeElapsed * 2.0 * 3.1415927410125732 * ((double) this.baseData.speed / ((double) this.convergeDistance * 2.0))))));
      this.LastVelocity = this.specRigidbody.Velocity;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

