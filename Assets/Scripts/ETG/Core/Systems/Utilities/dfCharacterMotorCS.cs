// Decompiled with JetBrains decompiler
// Type: dfCharacterMotorCS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [AddComponentMenu("Character/Character Motor (CSharp)")]
    [RequireComponent(typeof (CharacterController))]
    public class dfCharacterMotorCS : MonoBehaviour
    {
      public bool canControl = true;
      public bool useFixedUpdate = true;
      [NonSerialized]
      public Vector3 inputMoveDirection = Vector3.zero;
      [NonSerialized]
      public bool inputJump;
      public bool inputSprint;
      public dfCharacterMotorCS.CharacterMotorMovement movement = new dfCharacterMotorCS.CharacterMotorMovement();
      public dfCharacterMotorCS.CharacterMotorJumping jumping = new dfCharacterMotorCS.CharacterMotorJumping();
      public dfCharacterMotorCS.CharacterMotorMovingPlatform movingPlatform = new dfCharacterMotorCS.CharacterMotorMovingPlatform();
      public dfCharacterMotorCS.CharacterMotorSliding sliding = new dfCharacterMotorCS.CharacterMotorSliding();
      [NonSerialized]
      public bool grounded = true;
      [NonSerialized]
      public Vector3 groundNormal = Vector3.zero;
      private Vector3 lastGroundNormal = Vector3.zero;
      private Transform tr;
      private CharacterController controller;

      private void Awake()
      {
        this.controller = this.GetComponent<CharacterController>();
        this.tr = this.transform;
      }

      private void UpdateFunction()
      {
        Vector3 vector3 = this.ApplyGravityAndJumping(this.ApplyInputVelocityChange(this.movement.velocity));
        Vector3 zero = Vector3.zero;
        if (this.MoveWithPlatform())
        {
          Vector3 motion = this.movingPlatform.activePlatform.TransformPoint(this.movingPlatform.activeLocalPoint) - this.movingPlatform.activeGlobalPoint;
          if (motion != Vector3.zero)
          {
            int num = (int) this.controller.Move(motion);
          }
          float y = (this.movingPlatform.activePlatform.rotation * this.movingPlatform.activeLocalRotation * Quaternion.Inverse(this.movingPlatform.activeGlobalRotation)).eulerAngles.y;
          if ((double) y != 0.0)
            this.tr.Rotate(0.0f, y, 0.0f);
        }
        Vector3 position = this.tr.position;
        Vector3 motion1 = vector3 * BraveTime.DeltaTime;
        float num1 = Mathf.Max(this.controller.stepOffset, new Vector3(motion1.x, 0.0f, motion1.z).magnitude);
        if (this.grounded)
          motion1 -= num1 * Vector3.up;
        this.movingPlatform.hitPlatform = (Transform) null;
        this.groundNormal = Vector3.zero;
        this.movement.collisionFlags = this.controller.Move(motion1);
        this.movement.lastHitPoint = this.movement.hitPoint;
        this.lastGroundNormal = this.groundNormal;
        if (this.movingPlatform.enabled && (UnityEngine.Object) this.movingPlatform.activePlatform != (UnityEngine.Object) this.movingPlatform.hitPlatform && (UnityEngine.Object) this.movingPlatform.hitPlatform != (UnityEngine.Object) null)
        {
          this.movingPlatform.activePlatform = this.movingPlatform.hitPlatform;
          this.movingPlatform.lastMatrix = this.movingPlatform.hitPlatform.localToWorldMatrix;
          this.movingPlatform.newPlatform = true;
        }
        Vector3 rhs = new Vector3(vector3.x, 0.0f, vector3.z);
        this.movement.velocity = (this.tr.position - position) / BraveTime.DeltaTime;
        Vector3 lhs = new Vector3(this.movement.velocity.x, 0.0f, this.movement.velocity.z);
        if (rhs == Vector3.zero)
        {
          this.movement.velocity = new Vector3(0.0f, this.movement.velocity.y, 0.0f);
        }
        else
        {
          float num2 = Vector3.Dot(lhs, rhs) / rhs.sqrMagnitude;
          this.movement.velocity = rhs * Mathf.Clamp01(num2) + this.movement.velocity.y * Vector3.up;
        }
        if ((double) this.movement.velocity.y < (double) vector3.y - 0.001)
        {
          if ((double) this.movement.velocity.y < 0.0)
            this.movement.velocity.y = vector3.y;
          else
            this.jumping.holdingJumpButton = false;
        }
        if (this.grounded && !this.IsGroundedTest())
        {
          this.grounded = false;
          if (this.movingPlatform.enabled && (this.movingPlatform.movementTransfer == dfCharacterMotorCS.MovementTransferOnJump.InitTransfer || this.movingPlatform.movementTransfer == dfCharacterMotorCS.MovementTransferOnJump.PermaTransfer))
          {
            this.movement.frameVelocity = this.movingPlatform.platformVelocity;
            this.movement.velocity += this.movingPlatform.platformVelocity;
          }
          this.SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
          this.tr.position += num1 * Vector3.up;
        }
        else if (!this.grounded && this.IsGroundedTest())
        {
          this.grounded = true;
          this.jumping.jumping = false;
          this.SubtractNewPlatformVelocity();
          this.SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
        }
        if (!this.MoveWithPlatform())
          return;
        this.movingPlatform.activeGlobalPoint = this.tr.position + Vector3.up * (this.controller.center.y - this.controller.height * 0.5f + this.controller.radius);
        this.movingPlatform.activeLocalPoint = this.movingPlatform.activePlatform.InverseTransformPoint(this.movingPlatform.activeGlobalPoint);
        this.movingPlatform.activeGlobalRotation = this.tr.rotation;
        this.movingPlatform.activeLocalRotation = Quaternion.Inverse(this.movingPlatform.activePlatform.rotation) * this.movingPlatform.activeGlobalRotation;
      }

      private void FixedUpdate()
      {
        if (this.movingPlatform.enabled)
        {
          if ((UnityEngine.Object) this.movingPlatform.activePlatform != (UnityEngine.Object) null)
          {
            if (!this.movingPlatform.newPlatform)
              this.movingPlatform.platformVelocity = (this.movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocalPoint) - this.movingPlatform.lastMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocalPoint)) / BraveTime.DeltaTime;
            this.movingPlatform.lastMatrix = this.movingPlatform.activePlatform.localToWorldMatrix;
            this.movingPlatform.newPlatform = false;
          }
          else
            this.movingPlatform.platformVelocity = Vector3.zero;
        }
        if (!this.useFixedUpdate)
          return;
        this.UpdateFunction();
      }

      private void Update()
      {
        if (this.useFixedUpdate)
          return;
        this.UpdateFunction();
      }

      private Vector3 ApplyInputVelocityChange(Vector3 velocity)
      {
        if (!this.canControl)
          this.inputMoveDirection = Vector3.zero;
        Vector3 hVelocity;
        if (this.grounded && this.TooSteep())
        {
          Vector3 normalized = new Vector3(this.groundNormal.x, 0.0f, this.groundNormal.z).normalized;
          Vector3 vector3 = Vector3.Project(this.inputMoveDirection, normalized);
          hVelocity = (normalized + vector3 * this.sliding.speedControl + (this.inputMoveDirection - vector3) * this.sliding.sidewaysControl) * this.sliding.slidingSpeed;
        }
        else
          hVelocity = this.GetDesiredHorizontalVelocity();
        if (this.movingPlatform.enabled && this.movingPlatform.movementTransfer == dfCharacterMotorCS.MovementTransferOnJump.PermaTransfer)
        {
          hVelocity += this.movement.frameVelocity;
          hVelocity.y = 0.0f;
        }
        if (this.grounded)
          hVelocity = this.AdjustGroundVelocityToNormal(hVelocity, this.groundNormal);
        else
          velocity.y = 0.0f;
        float num = this.GetMaxAcceleration(this.grounded) * BraveTime.DeltaTime;
        Vector3 vector3_1 = hVelocity - velocity;
        if ((double) vector3_1.sqrMagnitude > (double) num * (double) num)
          vector3_1 = vector3_1.normalized * num;
        if (this.grounded || this.canControl)
          velocity += vector3_1;
        if (this.grounded)
          velocity.y = Mathf.Min(velocity.y, 0.0f);
        return velocity;
      }

      private Vector3 ApplyGravityAndJumping(Vector3 velocity)
      {
        if (!this.inputJump || !this.canControl)
        {
          this.jumping.holdingJumpButton = false;
          this.jumping.lastButtonDownTime = -100f;
        }
        if (this.inputJump && (double) this.jumping.lastButtonDownTime < 0.0 && this.canControl)
          this.jumping.lastButtonDownTime = UnityEngine.Time.time;
        if (this.grounded)
        {
          velocity.y = Mathf.Min(0.0f, velocity.y) - this.movement.gravity * BraveTime.DeltaTime;
        }
        else
        {
          velocity.y = this.movement.velocity.y - this.movement.gravity * BraveTime.DeltaTime;
          if (this.jumping.jumping && this.jumping.holdingJumpButton && (double) UnityEngine.Time.time < (double) this.jumping.lastStartTime + (double) this.jumping.extraHeight / (double) this.CalculateJumpVerticalSpeed(this.jumping.baseHeight))
            velocity += this.jumping.jumpDir * this.movement.gravity * BraveTime.DeltaTime;
          velocity.y = Mathf.Max(velocity.y, -this.movement.maxFallSpeed);
        }
        if (this.grounded)
        {
          if (this.jumping.enabled && this.canControl && (double) UnityEngine.Time.time - (double) this.jumping.lastButtonDownTime < 0.2)
          {
            this.grounded = false;
            this.jumping.jumping = true;
            this.jumping.lastStartTime = UnityEngine.Time.time;
            this.jumping.lastButtonDownTime = -100f;
            this.jumping.holdingJumpButton = true;
            this.jumping.jumpDir = !this.TooSteep() ? Vector3.Slerp(Vector3.up, this.groundNormal, this.jumping.perpAmount) : Vector3.Slerp(Vector3.up, this.groundNormal, this.jumping.steepPerpAmount);
            velocity.y = 0.0f;
            velocity += this.jumping.jumpDir * this.CalculateJumpVerticalSpeed(this.jumping.baseHeight);
            if (this.movingPlatform.enabled && (this.movingPlatform.movementTransfer == dfCharacterMotorCS.MovementTransferOnJump.InitTransfer || this.movingPlatform.movementTransfer == dfCharacterMotorCS.MovementTransferOnJump.PermaTransfer))
            {
              this.movement.frameVelocity = this.movingPlatform.platformVelocity;
              velocity += this.movingPlatform.platformVelocity;
            }
            this.SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
          }
          else
            this.jumping.holdingJumpButton = false;
        }
        return velocity;
      }

      private void OnControllerColliderHit(ControllerColliderHit hit)
      {
        if ((double) hit.normal.y <= 0.0 || (double) hit.normal.y <= (double) this.groundNormal.y || (double) hit.moveDirection.y >= 0.0)
          return;
        this.groundNormal = (double) (hit.point - this.movement.lastHitPoint).sqrMagnitude > 0.001 || this.lastGroundNormal == Vector3.zero ? hit.normal : this.lastGroundNormal;
        this.movingPlatform.hitPlatform = hit.collider.transform;
        this.movement.hitPoint = hit.point;
        this.movement.frameVelocity = Vector3.zero;
      }

      [DebuggerHidden]
      private IEnumerator SubtractNewPlatformVelocity()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new dfCharacterMotorCS.<SubtractNewPlatformVelocity>c__Iterator0()
        {
          $this = this
        };
      }

      private bool MoveWithPlatform()
      {
        return this.movingPlatform.enabled && (this.grounded || this.movingPlatform.movementTransfer == dfCharacterMotorCS.MovementTransferOnJump.PermaLocked) && (UnityEngine.Object) this.movingPlatform.activePlatform != (UnityEngine.Object) null;
      }

      private Vector3 GetDesiredHorizontalVelocity()
      {
        Vector3 desiredMovementDirection = this.tr.InverseTransformDirection(this.inputMoveDirection);
        float num = this.MaxSpeedInDirection(desiredMovementDirection);
        if (this.grounded)
        {
          float time = Mathf.Asin(this.movement.velocity.normalized.y) * 57.29578f;
          num *= this.movement.slopeSpeedMultiplier.Evaluate(time);
        }
        return this.tr.TransformDirection(desiredMovementDirection * num);
      }

      private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
      {
        return Vector3.Cross(Vector3.Cross(Vector3.up, hVelocity), groundNormal).normalized * hVelocity.magnitude;
      }

      private bool IsGroundedTest() => (double) this.groundNormal.y > 0.01;

      private float GetMaxAcceleration(bool grounded)
      {
        return grounded ? this.movement.maxGroundAcceleration : this.movement.maxAirAcceleration;
      }

      private float CalculateJumpVerticalSpeed(float targetJumpHeight)
      {
        return Mathf.Sqrt(2f * targetJumpHeight * this.movement.gravity);
      }

      private bool IsJumping() => this.jumping.jumping;

      private bool IsSliding() => this.grounded && this.sliding.enabled && this.TooSteep();

      private bool IsTouchingCeiling()
      {
        return (this.movement.collisionFlags & CollisionFlags.Above) != CollisionFlags.None;
      }

      private bool IsGrounded() => this.grounded;

      private bool TooSteep()
      {
        return (double) this.groundNormal.y <= (double) Mathf.Cos(this.controller.slopeLimit * ((float) Math.PI / 180f));
      }

      private Vector3 GetDirection() => this.inputMoveDirection;

      private void SetControllable(bool controllable) => this.canControl = controllable;

      private float MaxSpeedInDirection(Vector3 desiredMovementDirection)
      {
        if (desiredMovementDirection == Vector3.zero)
          return 0.0f;
        float num = ((double) desiredMovementDirection.z <= 0.0 ? this.movement.maxBackwardsSpeed : this.movement.maxForwardSpeed) / this.movement.maxSidewaysSpeed;
        Vector3 normalized = new Vector3(desiredMovementDirection.x, 0.0f, desiredMovementDirection.z / num).normalized;
        return new Vector3(normalized.x, 0.0f, normalized.z * num).magnitude * this.movement.maxSidewaysSpeed;
      }

      private void SetVelocity(Vector3 velocity)
      {
        this.grounded = false;
        this.movement.velocity = velocity;
        this.movement.frameVelocity = Vector3.zero;
        this.SendMessage("OnExternalVelocity");
      }

      [Serializable]
      public class CharacterMotorMovement
      {
        public float maxForwardSpeed = 3f;
        public float maxSidewaysSpeed = 2f;
        public float maxBackwardsSpeed = 2f;
        public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe[3]
        {
          new Keyframe(-90f, 1f),
          new Keyframe(0.0f, 1f),
          new Keyframe(90f, 0.0f)
        });
        public float maxGroundAcceleration = 30f;
        public float maxAirAcceleration = 20f;
        public float gravity = 9.81f;
        public float maxFallSpeed = 20f;
        [NonSerialized]
        public CollisionFlags collisionFlags;
        [NonSerialized]
        public Vector3 velocity;
        [NonSerialized]
        public Vector3 frameVelocity = Vector3.zero;
        [NonSerialized]
        public Vector3 hitPoint = Vector3.zero;
        [NonSerialized]
        public Vector3 lastHitPoint = new Vector3(float.PositiveInfinity, 0.0f, 0.0f);
      }

      public enum MovementTransferOnJump
      {
        None,
        InitTransfer,
        PermaTransfer,
        PermaLocked,
      }

      [Serializable]
      public class CharacterMotorJumping
      {
        public bool enabled = true;
        public float baseHeight = 1f;
        public float extraHeight = 4.1f;
        public float perpAmount;
        public float steepPerpAmount = 0.5f;
        [NonSerialized]
        public bool jumping;
        [NonSerialized]
        public bool holdingJumpButton;
        [NonSerialized]
        public float lastStartTime;
        [NonSerialized]
        public float lastButtonDownTime = -100f;
        [NonSerialized]
        public Vector3 jumpDir = Vector3.up;
      }

      [Serializable]
      public class CharacterMotorMovingPlatform
      {
        public bool enabled = true;
        public dfCharacterMotorCS.MovementTransferOnJump movementTransfer = dfCharacterMotorCS.MovementTransferOnJump.PermaTransfer;
        [NonSerialized]
        public Transform hitPlatform;
        [NonSerialized]
        public Transform activePlatform;
        [NonSerialized]
        public Vector3 activeLocalPoint;
        [NonSerialized]
        public Vector3 activeGlobalPoint;
        [NonSerialized]
        public Quaternion activeLocalRotation;
        [NonSerialized]
        public Quaternion activeGlobalRotation;
        [NonSerialized]
        public Matrix4x4 lastMatrix;
        [NonSerialized]
        public Vector3 platformVelocity;
        [NonSerialized]
        public bool newPlatform;
      }

      [Serializable]
      public class CharacterMotorSliding
      {
        public bool enabled = true;
        public float slidingSpeed = 15f;
        public float sidewaysControl = 1f;
        public float speedControl = 0.4f;
      }
    }

}
