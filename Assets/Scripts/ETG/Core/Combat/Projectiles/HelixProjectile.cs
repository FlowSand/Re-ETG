using UnityEngine;

#nullable disable

public class HelixProjectile : Projectile
  {
    public float helixWavelength = 3f;
    public float helixAmplitude = 1f;
    private bool m_helixInitialized;
    private Vector2 m_initialRightVector;
    private Vector2 m_initialUpVector;
    private Vector2 m_privateLastPosition;
    private float m_displacement;
    private float m_yDisplacement;

    public void AdjustRightVector(float angleDiff)
    {
      if (float.IsNaN(angleDiff))
        return;
      this.m_initialUpVector = (Vector2) (Quaternion.Euler(0.0f, 0.0f, angleDiff) * (Vector3) this.m_initialUpVector);
      this.m_initialRightVector = (Vector2) (Quaternion.Euler(0.0f, 0.0f, angleDiff) * (Vector3) this.m_initialRightVector);
    }

    protected override void Move()
    {
      if (!this.m_helixInitialized)
      {
        this.m_helixInitialized = true;
        this.m_initialRightVector = (Vector2) this.transform.right;
        this.m_initialUpVector = (Vector2) this.transform.up;
        this.m_privateLastPosition = this.sprite.WorldCenter;
        this.m_displacement = 0.0f;
        this.m_yDisplacement = 0.0f;
      }
      this.m_timeElapsed += BraveTime.DeltaTime;
      int num1 = !this.Inverted ? 1 : -1;
      float num2 = this.m_timeElapsed * this.baseData.speed;
      float num3 = (float) num1 * this.helixAmplitude * Mathf.Sin(this.m_timeElapsed * 3.14159274f * this.baseData.speed / this.helixWavelength);
      Vector2 vector2 = this.m_privateLastPosition + this.m_initialRightVector * (num2 - this.m_displacement) + this.m_initialUpVector * (num3 - this.m_yDisplacement);
      this.m_privateLastPosition = vector2;
      Vector2 v = (vector2 - this.sprite.WorldCenter) / BraveTime.DeltaTime;
      float num4 = BraveMathCollege.Atan2Degrees(v);
      if (this.shouldRotate && !float.IsNaN(num4))
        this.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, num4);
      if (!float.IsNaN(num4))
        this.m_currentDirection = v.normalized;
      this.m_displacement = num2;
      this.m_yDisplacement = num3;
      this.specRigidbody.Velocity = v;
      this.LastVelocity = this.specRigidbody.Velocity;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

