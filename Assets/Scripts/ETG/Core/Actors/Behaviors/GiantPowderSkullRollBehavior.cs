using FullInspector;
using System;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/GiantPowderSkull/RollBehavior")]
public class GiantPowderSkullRollBehavior : BasicAttackBehavior
  {
    public float[] startingAngles = new float[4]
    {
      45f,
      135f,
      225f,
      315f
    };
    public float rollSpeed = 9f;
    public int numBounces = 3;
    public BulletScriptSelector collisionBulletScript;
    [InspectorCategory("Visuals")]
    public PowderSkullParticleController trailParticleSystem;
    private GiantPowderSkullRollBehavior.RollState m_state;
    private float m_cachedVelocityFraction;
    private float m_timeSinceLastBounce;
    private int m_bounces;
    private float m_startingAngle;

    public override void Start()
    {
      base.Start();
      this.m_aiActor.specRigidbody.OnCollision += new Action<CollisionData>(this.OnCollision);
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.m_timeSinceLastBounce += this.m_deltaTime;
    }

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      this.m_startingAngle = BraveMathCollege.ClampAngle360(BraveUtility.RandomElement<float>(this.startingAngles));
      this.m_bounces = 0;
      if ((bool) (UnityEngine.Object) this.trailParticleSystem)
      {
        this.m_cachedVelocityFraction = this.trailParticleSystem.VelocityFraction;
        this.trailParticleSystem.VelocityFraction = 0.0f;
      }
      this.m_state = GiantPowderSkullRollBehavior.RollState.Charge;
      this.m_aiAnimator.LockFacingDirection = true;
      this.m_aiAnimator.FacingDirection = this.m_startingAngle;
      this.m_aiAnimator.PlayUntilFinished("roll_charge");
      this.m_aiActor.ClearPath();
      this.m_aiActor.BehaviorOverridesVelocity = true;
      this.m_aiActor.BehaviorVelocity = Vector2.zero;
      this.m_aiActor.specRigidbody.PixelColliders[0].Enabled = false;
      this.m_aiActor.specRigidbody.PixelColliders[1].Enabled = false;
      this.m_aiActor.specRigidbody.PixelColliders[3].Enabled = true;
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      if (this.m_state == GiantPowderSkullRollBehavior.RollState.Charge)
      {
        if (!this.m_aiAnimator.IsPlaying("roll_charge"))
        {
          this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_startingAngle, this.rollSpeed);
          this.m_aiAnimator.FacingDirection = this.m_aiActor.BehaviorVelocity.ToAngle();
          this.m_state = GiantPowderSkullRollBehavior.RollState.Rolling;
          this.m_aiAnimator.PlayUntilCancelled("roll");
        }
      }
      else if (this.m_state != GiantPowderSkullRollBehavior.RollState.Rolling && this.m_state == GiantPowderSkullRollBehavior.RollState.Stopping && !this.m_aiAnimator.IsPlaying("roll_out"))
        return ContinuousBehaviorResult.Finished;
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      this.m_aiAnimator.LockFacingDirection = false;
      if ((bool) (UnityEngine.Object) this.trailParticleSystem)
        this.trailParticleSystem.VelocityFraction = this.m_cachedVelocityFraction;
      this.m_aiActor.specRigidbody.PixelColliders[0].Enabled = true;
      this.m_aiActor.specRigidbody.PixelColliders[1].Enabled = true;
      this.m_aiActor.specRigidbody.PixelColliders[3].Enabled = false;
      this.m_state = GiantPowderSkullRollBehavior.RollState.None;
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    protected virtual void OnCollision(CollisionData collision)
    {
      if (this.m_state != GiantPowderSkullRollBehavior.RollState.Rolling || (bool) (UnityEngine.Object) collision.OtherRigidbody)
        return;
      if ((double) this.m_timeSinceLastBounce > 1.0)
      {
        ++this.m_bounces;
        SpawnManager.SpawnBulletScript((GameActor) this.m_aiActor, this.collisionBulletScript);
      }
      int num = (int) AkSoundEngine.PostEvent("Play_ENM_statue_stomp_01", this.m_aiActor.gameObject);
      if (this.m_bounces > this.numBounces)
      {
        PhysicsEngine.PostSliceVelocity = new Vector2?(Vector2.zero);
        this.m_aiActor.BehaviorVelocity = Vector2.zero;
        this.m_state = GiantPowderSkullRollBehavior.RollState.Stopping;
        this.m_aiAnimator.PlayUntilFinished("roll_stop");
      }
      else
      {
        Vector2 velocity = collision.MyRigidbody.Velocity;
        if (collision.CollidedX)
          velocity.x *= -1f;
        if (collision.CollidedY)
          velocity.y *= -1f;
        Vector2 vector = velocity.normalized * this.rollSpeed;
        this.m_aiAnimator.FacingDirection = vector.ToAngle();
        PhysicsEngine.PostSliceVelocity = new Vector2?(vector);
        this.m_aiActor.BehaviorVelocity = vector;
        this.m_timeSinceLastBounce = 0.0f;
      }
    }

    private enum RollState
    {
      None,
      Charge,
      Rolling,
      Stopping,
    }
  }

