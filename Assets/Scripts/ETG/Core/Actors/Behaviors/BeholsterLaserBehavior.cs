using System;

using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/Beholster/LaserBehavior")]
public class BeholsterLaserBehavior : BasicAttackBehavior
    {
        public BeholsterLaserBehavior.TrackingType trackingType;
        public float initialAimOffset;
        public float chargeTime;
        public float firingTime;
        public float maxTurnRate;
        public float turnRateAcceleration;
        public bool useDegreeCatchUp;
        [InspectorShowIf("useDegreeCatchUp")]
        [InspectorIndent]
        public float minDegreesForCatchUp;
        [InspectorIndent]
        [InspectorShowIf("useDegreeCatchUp")]
        public float degreeCatchUpSpeed;
        public bool useUnitCatchUp;
        [InspectorIndent]
        [InspectorShowIf("useUnitCatchUp")]
        public float minUnitForCatchUp;
        [InspectorIndent]
        [InspectorShowIf("useUnitCatchUp")]
        public float maxUnitForCatchUp;
        [InspectorShowIf("useUnitCatchUp")]
        [InspectorIndent]
        public float unitCatchUpSpeed;
        public bool useUnitOvershoot;
        [InspectorIndent]
        [InspectorShowIf("useUnitOvershoot")]
        public float minUnitForOvershoot;
        [InspectorShowIf("useUnitOvershoot")]
        [InspectorIndent]
        public float unitOvershootTime;
        [InspectorShowIf("useUnitOvershoot")]
        [InspectorIndent]
        public float unitOvershootSpeed;
        private BeholsterController m_beholsterController;
        private BeholsterLaserBehavior.State m_state;
        private float m_timer;
        private Vector2 m_targetPosition;
        private float m_currentUnitTurnRate;
        private float m_unitOvershootFixedDirection;
        private float m_unitOvershootTimer;
        private SpeculativeRigidbody m_backupTarget;

        public override void Start()
        {
            base.Start();
            this.m_beholsterController = this.m_aiActor.GetComponent<BeholsterController>();
        }

        public override void Upkeep()
        {
            base.Upkeep();
            if ((bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
            {
                this.m_targetPosition = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
                this.m_backupTarget = this.m_aiActor.TargetRigidbody;
            }
            else
            {
                if (!(bool) (UnityEngine.Object) this.m_backupTarget)
                    return;
                this.m_targetPosition = this.m_backupTarget.GetUnitCenter(ColliderType.HitBox);
            }
        }

        public override BehaviorResult Update()
        {
            int num = (int) base.Update();
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.IsReady())
                return BehaviorResult.Continue;
            this.m_aiActor.ClearPath();
            this.m_beholsterController.StopFiringTentacles();
            this.m_beholsterController.PrechargeFiringLaser();
            this.m_state = BeholsterLaserBehavior.State.PreCharging;
            this.m_aiActor.SuppressTargetSwitch = true;
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num1 = (int) base.ContinuousUpdate();
            Vector2 b1 = this.m_aiActor.transform.position.XY() + this.m_beholsterController.firingEllipseCenter;
            if (this.m_state == BeholsterLaserBehavior.State.PreCharging)
            {
                if (!this.m_aiActor.spriteAnimator.Playing)
                {
                    this.m_beholsterController.ChargeFiringLaser(this.chargeTime);
                    this.m_timer = this.chargeTime;
                    this.m_state = BeholsterLaserBehavior.State.Charging;
                }
            }
            else
            {
                if (this.m_state == BeholsterLaserBehavior.State.Charging)
                {
                    this.m_timer -= this.m_deltaTime;
                    if ((double) this.m_timer <= 0.0)
                    {
                        this.m_beholsterController.StartFiringLaser(this.m_aiActor.aiAnimator.FacingDirection);
                        this.m_timer = this.firingTime;
                        this.m_state = BeholsterLaserBehavior.State.Firing;
                    }
                    return ContinuousBehaviorResult.Continue;
                }
                if (this.m_state == BeholsterLaserBehavior.State.Firing)
                {
                    this.m_timer -= this.m_deltaTime;
                    if ((double) this.m_timer <= 0.0 || !this.m_beholsterController.FiringLaser)
                        return ContinuousBehaviorResult.Finished;
                    float num2;
                    if (this.trackingType == BeholsterLaserBehavior.TrackingType.Follow)
                    {
                        float num3 = Vector2.Distance(this.m_targetPosition, b1);
                        float f1 = BraveMathCollege.ClampAngle180((this.m_targetPosition - b1).ToAngle() - this.m_beholsterController.LaserAngle);
                        float f2 = (float) ((double) f1 * (double) num3 * (Math.PI / 180.0));
                        float max = this.maxTurnRate;
                        float num4 = Mathf.Sign(f1);
                        if ((double) this.m_unitOvershootTimer > 0.0)
                        {
                            num4 = this.m_unitOvershootFixedDirection;
                            this.m_unitOvershootTimer -= this.m_deltaTime;
                            max = this.unitOvershootSpeed;
                        }
                        this.m_currentUnitTurnRate = Mathf.Clamp(this.m_currentUnitTurnRate + num4 * this.turnRateAcceleration * this.m_deltaTime, -max, max);
                        float num5 = (float) ((double) this.m_currentUnitTurnRate / (double) num3 * 57.295780181884766);
                        float a = 0.0f;
                        if (this.useDegreeCatchUp && (double) Mathf.Abs(f1) > (double) this.minDegreesForCatchUp)
                        {
                            float b2 = Mathf.InverseLerp(this.minDegreesForCatchUp, 180f, Mathf.Abs(f1)) * this.degreeCatchUpSpeed;
                            a = Mathf.Max(a, b2);
                        }
                        if (this.useUnitCatchUp && (double) Mathf.Abs(f2) > (double) this.minUnitForCatchUp)
                        {
                            float b3 = (float) ((double) (Mathf.InverseLerp(this.minUnitForCatchUp, this.maxUnitForCatchUp, Mathf.Abs(f2)) * this.unitCatchUpSpeed) / (double) num3 * 57.295780181884766);
                            a = Mathf.Max(a, b3);
                        }
                        if (this.useUnitOvershoot && (double) Mathf.Abs(f2) < (double) this.minUnitForOvershoot)
                        {
                            this.m_unitOvershootFixedDirection = (double) this.m_currentUnitTurnRate <= 0.0 ? -1f : 1f;
                            this.m_unitOvershootTimer = this.unitOvershootTime;
                        }
                        float num6 = a * Mathf.Sign(f1);
                        num2 = BraveMathCollege.ClampAngle360(this.m_beholsterController.LaserAngle + (num5 + num6) * this.m_deltaTime);
                    }
                    else
                        num2 = BraveMathCollege.ClampAngle360(this.m_beholsterController.LaserAngle + this.maxTurnRate * this.m_deltaTime);
                    if ((bool) (UnityEngine.Object) this.m_beholsterController.LaserBeam && this.m_beholsterController.LaserBeam.State != BasicBeamController.BeamState.Charging)
                        this.m_beholsterController.LaserAngle = num2;
                    return ContinuousBehaviorResult.Continue;
                }
            }
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.m_beholsterController.StopFiringLaser();
            this.m_aiAnimator.LockFacingDirection = false;
            this.m_aiActor.SuppressTargetSwitch = false;
            this.m_updateEveryFrame = false;
            this.UpdateCooldowns();
        }

        public override bool IsOverridable() => false;

        public enum State
        {
            PreCharging,
            Charging,
            Firing,
        }

        public enum TrackingType
        {
            Follow,
            ConstantTurn,
        }
    }

