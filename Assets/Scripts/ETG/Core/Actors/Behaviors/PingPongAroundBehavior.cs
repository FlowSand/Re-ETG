using System;
using UnityEngine;

#nullable disable

public class PingPongAroundBehavior : MovementBehaviorBase
  {
    public float[] startingAngles = new float[4]
    {
      45f,
      135f,
      225f,
      315f
    };
    public PingPongAroundBehavior.MotionType motionType = PingPongAroundBehavior.MotionType.Diagonals;
    private bool m_isBouncing;
    private float m_startingAngle;

    private bool ReflectX
    {
      get
      {
        return this.motionType == PingPongAroundBehavior.MotionType.Diagonals || this.motionType == PingPongAroundBehavior.MotionType.Horizontal;
      }
    }

    private bool ReflectY
    {
      get
      {
        return this.motionType == PingPongAroundBehavior.MotionType.Diagonals || this.motionType == PingPongAroundBehavior.MotionType.Vertical;
      }
    }

    public override void Start()
    {
      base.Start();
      this.m_aiActor.specRigidbody.OnCollision += new Action<CollisionData>(this.OnCollision);
    }

    public override BehaviorResult Update()
    {
      this.m_startingAngle = BraveMathCollege.ClampAngle360(BraveUtility.RandomElement<float>(this.startingAngles));
      this.m_aiActor.BehaviorOverridesVelocity = true;
      this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_startingAngle, this.m_aiActor.MovementSpeed);
      this.m_isBouncing = true;
      return BehaviorResult.RunContinuousInClass;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      return !this.m_aiActor.BehaviorOverridesVelocity ? ContinuousBehaviorResult.Finished : ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      this.m_isBouncing = false;
    }

    protected virtual void OnCollision(CollisionData collision)
    {
      if (!this.m_isBouncing || (bool) (UnityEngine.Object) collision.OtherRigidbody && (bool) (UnityEngine.Object) collision.OtherRigidbody.projectile || !collision.CollidedX && !collision.CollidedY)
        return;
      Vector2 velocity = collision.MyRigidbody.Velocity;
      if (collision.CollidedX && this.ReflectX)
        velocity.x *= -1f;
      if (collision.CollidedY && this.ReflectY)
        velocity.y *= -1f;
      if (this.motionType == PingPongAroundBehavior.MotionType.Horizontal)
        velocity.y = 0.0f;
      if (this.motionType == PingPongAroundBehavior.MotionType.Vertical)
        velocity.x = 0.0f;
      Vector2 vector2 = velocity.normalized * this.m_aiActor.MovementSpeed;
      PhysicsEngine.PostSliceVelocity = new Vector2?(vector2);
      this.m_aiActor.BehaviorVelocity = vector2;
    }

    public enum MotionType
    {
      Diagonals = 10, // 0x0000000A
      Horizontal = 20, // 0x00000014
      Vertical = 30, // 0x0000001E
    }
  }

