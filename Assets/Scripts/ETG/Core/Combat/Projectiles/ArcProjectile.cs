using UnityEngine;

#nullable disable

public class ArcProjectile : Projectile
  {
    [Header("Arc Projectile")]
    public float startingHeight = 1f;
    public float startingZSpeed;
    public float gravity = -10f;
    public bool destroyOnGroundContact = true;
    public string groundAudioEvent = string.Empty;
    public GameObject LandingTargetSprite;
    private float m_currentHeight;
    private Vector3 m_current3DVelocity;
    private tk2dBaseSprite m_landingTarget;
    private Vector3 m_targetLandPosition;

    public event System.Action OnGrounded;

    public override void Start()
    {
      base.Start();
      this.m_currentHeight = this.startingHeight;
      this.m_current3DVelocity = (this.m_currentDirection * this.m_currentSpeed).ToVector3ZUp(this.startingZSpeed);
      if (!(bool) (UnityEngine.Object) this.LandingTargetSprite || (bool) (UnityEngine.Object) this.m_landingTarget)
        return;
      float timeInFlight = this.GetTimeInFlight();
      this.m_targetLandPosition = (Vector3) (this.transform.position.XY() + this.m_currentDirection * this.m_currentSpeed * timeInFlight + new Vector2(0.0f, -this.m_currentHeight));
      this.m_landingTarget = SpawnManager.SpawnVFX(this.LandingTargetSprite, this.m_targetLandPosition, Quaternion.identity).GetComponent<tk2dBaseSprite>();
      this.m_landingTarget.UpdateZDepth();
      tk2dSpriteAnimator componentInChildren = this.m_landingTarget.GetComponentInChildren<tk2dSpriteAnimator>();
      componentInChildren.Play(componentInChildren.DefaultClip, 0.0f, (float) componentInChildren.DefaultClip.frames.Length / timeInFlight);
    }

    protected override void OnDestroy() => base.OnDestroy();

    protected override void Move()
    {
      if ((double) this.angularVelocity != 0.0)
        this.transform.RotateAround((Vector3) this.sprite.WorldCenter, Vector3.forward, this.angularVelocity * this.LocalDeltaTime);
      this.m_current3DVelocity.x = this.m_currentDirection.x;
      this.m_current3DVelocity.y = this.m_currentDirection.y;
      this.m_current3DVelocity.z += this.LocalDeltaTime * this.gravity;
      float num1 = this.m_currentHeight + this.m_current3DVelocity.z * this.LocalDeltaTime;
      if ((double) num1 < 0.0)
      {
        if (this.OnGrounded != null)
          this.OnGrounded();
        if (!string.IsNullOrEmpty(this.groundAudioEvent))
        {
          int num2 = (int) AkSoundEngine.PostEvent(this.groundAudioEvent, this.gameObject);
        }
        if (this.destroyOnGroundContact)
        {
          this.DieInAir();
        }
        else
        {
          this.m_current3DVelocity.z = -this.m_current3DVelocity.z;
          num1 = this.m_currentHeight + this.m_current3DVelocity.z * this.LocalDeltaTime;
        }
      }
      this.m_currentHeight = num1;
      this.m_currentDirection = this.m_current3DVelocity.XY();
      Vector2 vector2 = this.m_current3DVelocity.XY().normalized * this.m_currentSpeed;
      this.specRigidbody.Velocity = new Vector2(vector2.x, vector2.y + this.m_current3DVelocity.z);
      this.LastVelocity = this.m_current3DVelocity.XY();
      this.UpdateTargetPosition(false);
    }

    public float GetTimeInFlight()
    {
      float num = -this.startingHeight;
      float startingZspeed = this.startingZSpeed;
      float gravity = this.gravity;
      float timeInFlight = (float) (((double) Mathf.Sqrt((float) (2.0 * (double) gravity * (double) num + (double) startingZspeed * (double) startingZspeed)) + (double) startingZspeed) / -(double) gravity);
      if ((double) timeInFlight < 0.0)
        timeInFlight = (Mathf.Sqrt((float) (2.0 * (double) gravity * (double) num + (double) startingZspeed * (double) startingZspeed)) - startingZspeed) / gravity;
      return timeInFlight;
    }

    public float GetRemainingTimeInFlight()
    {
      float num = -this.m_currentHeight;
      float z = this.m_current3DVelocity.z;
      float gravity = this.gravity;
      float remainingTimeInFlight = (float) (((double) Mathf.Sqrt((float) (2.0 * (double) gravity * (double) num + (double) z * (double) z)) + (double) z) / -(double) gravity);
      if ((double) remainingTimeInFlight < 0.0)
        remainingTimeInFlight = (Mathf.Sqrt((float) (2.0 * (double) gravity * (double) num + (double) z * (double) z)) - z) / gravity;
      return remainingTimeInFlight;
    }

    public override float EstimatedTimeToTarget(Vector2 targetPoint, Vector2? overridePos = null)
    {
      return this.GetTimeInFlight();
    }

    public override Vector2 GetPredictedTargetPosition(
      Vector2 targetCenter,
      Vector2 targetVelocity,
      Vector2? overridePos = null,
      float? overrideProjectileSpeed = null)
    {
      return BraveMathCollege.GetPredictedPosition(targetCenter, targetVelocity, this.EstimatedTimeToTarget(targetCenter, overridePos));
    }

    public void AdjustSpeedToHit(Vector2 target)
    {
      this.baseData.speed = (target - this.transform.position.XY()).magnitude / this.GetTimeInFlight();
      this.UpdateSpeed();
      this.UpdateTargetPosition(true);
    }

    protected override void HandleDestruction(
      CollisionData lcr,
      bool allowActorSpawns = true,
      bool allowProjectileSpawns = true)
    {
      if ((bool) (UnityEngine.Object) this.m_landingTarget)
      {
        SpawnManager.Despawn(this.m_landingTarget.gameObject);
        this.m_landingTarget = (tk2dBaseSprite) null;
      }
      base.HandleDestruction(lcr, allowActorSpawns, allowProjectileSpawns);
    }

    public override void OnDespawned()
    {
      if ((bool) (UnityEngine.Object) this.m_landingTarget)
      {
        SpawnManager.Despawn(this.m_landingTarget.gameObject);
        this.m_landingTarget = (tk2dBaseSprite) null;
      }
      base.OnDespawned();
    }

    private void UpdateTargetPosition(bool useStartPosition)
    {
      if (!(bool) (UnityEngine.Object) this.LandingTargetSprite || !(bool) (UnityEngine.Object) this.m_landingTarget)
        return;
      float num = !useStartPosition ? this.GetRemainingTimeInFlight() : this.GetTimeInFlight();
      this.m_targetLandPosition = (Vector3) Vector2.Lerp((Vector2) this.m_targetLandPosition, this.transform.position.XY() + this.m_currentDirection * this.m_currentSpeed * num + new Vector2(0.0f, -this.m_currentHeight), !useStartPosition ? Mathf.Clamp01(BraveTime.DeltaTime * 4f) : 1f);
      this.m_landingTarget.transform.position = this.m_targetLandPosition;
      this.m_landingTarget.UpdateZDepth();
    }
  }

