// Decompiled with JetBrains decompiler
// Type: GrenadeProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class GrenadeProjectile : Projectile
    {
      public float startingHeight = 1f;
      private float m_currentHeight;
      private Vector3 m_current3DVelocity;

      public override void Start()
      {
        base.Start();
        this.m_currentHeight = this.startingHeight;
        this.m_current3DVelocity = (this.m_currentDirection * this.m_currentSpeed).ToVector3ZUp();
      }

      protected override void OnDestroy() => base.OnDestroy();

      protected override void Move()
      {
        this.m_current3DVelocity.x = this.m_currentDirection.x;
        this.m_current3DVelocity.y = this.m_currentDirection.y;
        this.m_current3DVelocity.z += this.LocalDeltaTime * -10f;
        float num = this.m_currentHeight + this.m_current3DVelocity.z * this.LocalDeltaTime;
        if ((double) num < 0.0)
        {
          this.m_current3DVelocity.z = -this.m_current3DVelocity.z;
          num *= -1f;
        }
        this.m_currentHeight = num;
        this.m_currentDirection = this.m_current3DVelocity.XY();
        Vector2 vector2 = this.m_current3DVelocity.XY().normalized * this.m_currentSpeed;
        this.specRigidbody.Velocity = new Vector2(vector2.x, vector2.y + this.m_current3DVelocity.z);
        this.LastVelocity = this.m_current3DVelocity.XY();
      }

      protected override void DoModifyVelocity()
      {
        if (this.ModifyVelocity == null)
          return;
        Vector2 vector2 = this.ModifyVelocity(this.m_current3DVelocity.XY().normalized * this.m_currentSpeed);
        this.specRigidbody.Velocity = new Vector2(vector2.x, vector2.y + this.m_current3DVelocity.z);
        if ((double) vector2.sqrMagnitude <= 0.0)
          return;
        this.m_currentDirection = vector2.normalized;
      }
    }

}
