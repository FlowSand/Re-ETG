using System;
using System.Collections.Generic;

using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/MetalGearRat/BeamsBehavior")]
public class MetalGearRatBeamsBehavior : BasicAttackBehavior
    {
        public List<AIBeamShooter> specificBeamShooters;
        public float firingTime;
        public bool stopWhileFiring;
        public float turnTime = 1f;
        public float slitherPeriod;
        public float slitherMagnitude;
        public float targetMoveSpeed = 3f;
        public float targetMoveAcceleration = 0.25f;
        public int randomTargets = 2;
        public float randomRetargetMin = 1f;
        public float randomRetargetMax = 2f;
        public BulletScriptSelector BulletScript;
        public Transform ShootPoint;
        [InspectorCategory("Visuals")]
        public string TellAnimation;
        [InspectorCategory("Visuals")]
        public string FireAnimation;
        [InspectorCategory("Visuals")]
        public string PostFireAnimation;
        private MetalGearRatBeamsBehavior.TargetData[] m_targetData;
        private float m_timer;
        private float m_slitherCounter;
        private float m_moveSpeed;
        private Vector2 m_roomLowerLeft;
        private Vector2 m_roomUpperRight;
        private BulletScriptSource m_bulletSource;
        private MetalGearRatBeamsBehavior.State m_state;

        public override void Start()
        {
            base.Start();
            if (!string.IsNullOrEmpty(this.TellAnimation))
            {
                this.m_aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered);
                if ((bool) (UnityEngine.Object) this.m_aiAnimator.ChildAnimator)
                    this.m_aiAnimator.ChildAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered);
            }
            this.m_roomLowerLeft = this.m_aiActor.ParentRoom.area.UnitBottomLeft;
            this.m_roomUpperRight = this.m_aiActor.ParentRoom.area.UnitTopRight + new Vector2(0.0f, 3f);
        }

        public override void Upkeep()
        {
            base.Upkeep();
            this.m_slitherCounter += this.m_deltaTime * this.m_aiActor.behaviorSpeculator.CooldownScale;
        }

        public override BehaviorResult Update()
        {
            int num = (int) base.Update();
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.IsReady())
                return BehaviorResult.Continue;
            if (!string.IsNullOrEmpty(this.TellAnimation))
            {
                this.m_aiAnimator.PlayUntilFinished(this.TellAnimation, true);
                this.state = MetalGearRatBeamsBehavior.State.WaitingForTell;
            }
            else
                this.Fire();
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num = (int) base.ContinuousUpdate();
            if (this.state == MetalGearRatBeamsBehavior.State.WaitingForTell)
            {
                if (!this.m_aiAnimator.IsPlaying(this.TellAnimation))
                    this.Fire();
                return ContinuousBehaviorResult.Continue;
            }
            if (this.state == MetalGearRatBeamsBehavior.State.Firing)
            {
                this.m_timer -= this.m_deltaTime;
                this.m_moveSpeed += this.targetMoveAcceleration * this.m_deltaTime;
                if ((double) this.m_timer <= 0.0 || !this.specificBeamShooters[0].IsFiringLaser)
                {
                    this.StopLasers();
                    if (string.IsNullOrEmpty(this.PostFireAnimation))
                        return ContinuousBehaviorResult.Finished;
                    this.state = MetalGearRatBeamsBehavior.State.WaitingForPostAnim;
                    this.m_aiAnimator.PlayUntilFinished(this.PostFireAnimation);
                    return ContinuousBehaviorResult.Continue;
                }
                for (int index = 0; index < this.specificBeamShooters.Count; ++index)
                {
                    AIBeamShooter specificBeamShooter = this.specificBeamShooters[index];
                    Vector2? nullable = new Vector2?();
                    if (this.m_targetData[index].hasFixedTarget)
                    {
                        nullable = new Vector2?(this.m_targetData[index].fixedTarget);
                        this.m_targetData[index].fixedTargetTimer -= this.m_deltaTime;
                        if ((double) this.m_targetData[index].fixedTargetTimer <= 0.0)
                        {
                            this.m_targetData[index].fixedTarget = this.RandomTargetPosition();
                            this.m_targetData[index].fixedTargetTimer = UnityEngine.Random.Range(this.randomRetargetMin, this.randomRetargetMax);
                        }
                    }
                    else if ((bool) (UnityEngine.Object) this.m_targetData[index].targetRigidbody)
                        nullable = new Vector2?(this.m_targetData[index].targetRigidbody.GetUnitCenter(ColliderType.HitBox));
                    if (nullable.HasValue)
                    {
                        Vector2 pos = this.m_targetData[index].pos;
                        float angle = (nullable.Value - pos).ToAngle();
                        this.m_targetData[index].direction = Mathf.SmoothDampAngle(this.m_targetData[index].direction, angle, ref this.m_targetData[index].angularVelocity, this.turnTime);
                    }
                    this.m_targetData[index].slitherDirection = Mathf.Sin(this.m_slitherCounter * 3.14159274f / this.slitherPeriod) * this.slitherMagnitude;
                    Vector2 vector1 = BraveMathCollege.DegreesToVector(this.m_targetData[index].direction + this.m_targetData[index].slitherDirection, this.m_moveSpeed);
                    this.m_targetData[index].pos += vector1 * this.m_deltaTime;
                    this.m_targetData[index].pos = Vector2Extensions.Clamp(this.m_targetData[index].pos, this.m_roomLowerLeft, this.m_roomUpperRight);
                    Vector2 vector2 = this.m_targetData[index].pos - specificBeamShooter.LaserFiringCenter;
                    specificBeamShooter.LaserAngle = vector2.ToAngle();
                    specificBeamShooter.MaxBeamLength = vector2.magnitude;
                }
                return ContinuousBehaviorResult.Continue;
            }
            return this.state == MetalGearRatBeamsBehavior.State.WaitingForPostAnim && !this.m_aiAnimator.IsPlaying(this.PostFireAnimation) ? ContinuousBehaviorResult.Finished : ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            if (!string.IsNullOrEmpty(this.TellAnimation))
                this.m_aiAnimator.EndAnimationIf(this.TellAnimation);
            if (!string.IsNullOrEmpty(this.FireAnimation))
                this.m_aiAnimator.EndAnimationIf(this.FireAnimation);
            if (!string.IsNullOrEmpty(this.PostFireAnimation))
                this.m_aiAnimator.EndAnimationIf(this.PostFireAnimation);
            this.StopLasers();
            this.m_aiAnimator.LockFacingDirection = false;
            this.m_updateEveryFrame = false;
            this.UpdateCooldowns();
        }

        public override void OnActorPreDeath()
        {
            base.OnActorPreDeath();
            this.StopLasers();
        }

        private void AnimEventTriggered(
            tk2dSpriteAnimator sprite,
            tk2dSpriteAnimationClip clip,
            int frameNum)
        {
            tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
            if (this.state != MetalGearRatBeamsBehavior.State.WaitingForTell || !(frame.eventInfo == "fire"))
                return;
            this.Fire();
        }

        private void Fire()
        {
            this.m_moveSpeed = this.targetMoveSpeed;
            this.m_slitherCounter = 0.0f;
            if (!string.IsNullOrEmpty(this.FireAnimation))
            {
                this.m_aiAnimator.EndAnimation();
                this.m_aiAnimator.PlayUntilFinished(this.FireAnimation);
            }
            if (this.stopWhileFiring)
                this.m_aiActor.ClearPath();
            this.m_targetData = new MetalGearRatBeamsBehavior.TargetData[this.specificBeamShooters.Count];
            for (int index = 0; index < this.specificBeamShooters.Count; ++index)
            {
                AIBeamShooter specificBeamShooter = this.specificBeamShooters[index];
                specificBeamShooter.IgnoreAiActorPlayerChecks = true;
                Vector2 vector2 = this.RandomTargetPosition();
                this.m_targetData[index] = new MetalGearRatBeamsBehavior.TargetData()
                {
                    pos = vector2,
                    direction = BraveUtility.RandomAngle()
                };
                Vector2 vector = vector2 - specificBeamShooter.LaserFiringCenter;
                specificBeamShooter.MaxBeamLength = vector.magnitude;
                if (index < this.randomTargets)
                {
                    this.m_targetData[index].hasFixedTarget = true;
                    this.m_targetData[index].fixedTarget = this.RandomTargetPosition();
                    this.m_targetData[index].fixedTargetTimer = UnityEngine.Random.Range(this.randomRetargetMin, this.randomRetargetMax);
                }
                else
                {
                    PlayerController randomActivePlayer = GameManager.Instance.GetRandomActivePlayer();
                    if ((bool) (UnityEngine.Object) randomActivePlayer && (bool) (UnityEngine.Object) randomActivePlayer.specRigidbody)
                        this.m_targetData[index].targetRigidbody = randomActivePlayer.specRigidbody;
                }
                specificBeamShooter.StartFiringLaser(vector.ToAngle());
            }
            this.m_timer = this.firingTime;
            if (!(bool) (UnityEngine.Object) this.m_bulletSource)
                this.m_bulletSource = this.ShootPoint.gameObject.GetOrAddComponent<BulletScriptSource>();
            this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
            this.m_bulletSource.BulletScript = this.BulletScript;
            this.m_bulletSource.Initialize();
            this.state = MetalGearRatBeamsBehavior.State.Firing;
        }

        private void StopLasers()
        {
            for (int index = 0; index < this.specificBeamShooters.Count; ++index)
                this.specificBeamShooters[index].StopFiringLaser();
            if (!(bool) (UnityEngine.Object) this.m_bulletSource)
                return;
            this.m_bulletSource.ForceStop();
        }

        private Vector2 RandomTargetPosition()
        {
            return BraveUtility.RandomVector2(this.m_roomLowerLeft + new Vector2(1f, 3f), this.m_roomUpperRight.WithY(this.m_aiActor.transform.position.y) - new Vector2(1f, 0.0f));
        }

        private MetalGearRatBeamsBehavior.State state
        {
            get => this.m_state;
            set
            {
                if (this.m_state == value)
                    return;
                this.EndState(this.m_state);
                this.m_state = value;
                this.BeginState(this.m_state);
            }
        }

        private void BeginState(MetalGearRatBeamsBehavior.State state)
        {
        }

        private void EndState(MetalGearRatBeamsBehavior.State state)
        {
        }

        private struct TargetData
        {
            public Vector2 pos;
            public float slitherCounter;
            public float direction;
            public float slitherDirection;
            public float angularVelocity;
            public bool hasFixedTarget;
            public Vector2 fixedTarget;
            public float fixedTargetTimer;
            public SpeculativeRigidbody targetRigidbody;
        }

        private enum State
        {
            Idle,
            WaitingForTell,
            Firing,
            WaitingForPostAnim,
        }
    }

