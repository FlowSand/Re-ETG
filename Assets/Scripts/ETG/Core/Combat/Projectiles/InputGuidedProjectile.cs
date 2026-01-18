using UnityEngine;

#nullable disable

public class InputGuidedProjectile : Projectile
  {
    [Header("Input Guiding")]
    public float trackingSpeed = 45f;
    public float dumbfireTime;
    private float m_dumbfireTimer;

    protected override void Move()
    {
      bool flag = true;
      if ((double) this.dumbfireTime > 0.0 && (double) this.m_dumbfireTimer < (double) this.dumbfireTime)
      {
        this.m_dumbfireTimer += BraveTime.DeltaTime;
        flag = false;
      }
      if (flag && this.Owner is PlayerController)
      {
        BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer((this.Owner as PlayerController).PlayerIDX);
        Vector2 zero = Vector2.zero;
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.MoveTowardsAngle(this.transform.eulerAngles.z, (!instanceForPlayer.IsKeyboardAndMouse() ? instanceForPlayer.ActiveActions.Aim.Vector : (this.Owner as PlayerController).unadjustedAimPoint.XY() - this.specRigidbody.UnitCenter).ToAngle(), this.trackingSpeed * BraveTime.DeltaTime));
      }
      this.specRigidbody.Velocity = (Vector2) (this.transform.right * this.baseData.speed);
      this.LastVelocity = this.specRigidbody.Velocity;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

