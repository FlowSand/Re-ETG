using System;
using UnityEngine;

#nullable disable

public class OrbitingSubprojectile : Projectile
  {
    public float RotationPeriod = 1f;
    public float RotationRadius = 2f;
    [NonSerialized]
    public Projectile TargetMainProjectile;
    private float m_elapsed;

    public void AssignProjectile(Projectile p) => this.TargetMainProjectile = p;

    protected override void Move()
    {
      if (!(bool) (UnityEngine.Object) this.TargetMainProjectile)
      {
        base.Move();
      }
      else
      {
        this.m_elapsed += BraveTime.DeltaTime;
        this.specRigidbody.Velocity = (this.TargetMainProjectile.specRigidbody.UnitCenter + (Vector2) ((Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(0.0f, 360f, this.m_elapsed % this.RotationPeriod / this.RotationPeriod)) * (Vector3) Vector2.right).normalized * this.RotationRadius) - (this.specRigidbody.UnitCenter - this.transform.position.XY()) - this.transform.position.XY()) / BraveTime.DeltaTime;
        this.LastVelocity = this.specRigidbody.Velocity;
      }
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

