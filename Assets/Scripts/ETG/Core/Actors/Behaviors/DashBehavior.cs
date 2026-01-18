using System;

using FullInspector;
using UnityEngine;

#nullable disable

public class DashBehavior : BasicAttackBehavior
    {
        public DashBehavior.DashDirection dashDirection = DashBehavior.DashDirection.Random;
        public float quantizeDirection;
        public float dashDistance;
        public float dashTime;
        public float postDashSpeed;
        public float doubleDashChance;
        public bool avoidTarget;
        [InspectorCategory("Attack")]
        public GameObject ShootPoint;
        [InspectorCategory("Attack")]
        public BulletScriptSelector bulletScript;
        [InspectorCategory("Attack")]
        public bool fireAtDashStart;
        [InspectorCategory("Attack")]
        public bool stopOnCollision;
        [InspectorCategory("Visuals")]
        public string chargeAnim;
        [InspectorCategory("Visuals")]
        public string dashAnim;
        [InspectorCategory("Visuals")]
        public bool doDodgeDustUp;
        [InspectorCategory("Visuals")]
        public bool warpDashAnimLength = true;
        [InspectorCategory("Visuals")]
        public bool hideShadow;
        [InspectorCategory("Visuals")]
        public bool hideGun;
        [InspectorCategory("Visuals")]
        public bool toggleTrailRenderer;
        [InspectorCategory("Visuals")]
        public bool enableShadowTrail;
        private tk2dBaseSprite m_shadowSprite;
        private TrailRenderer m_trailRenderer;
        private AfterImageTrailController m_shadowTrail;
        private BulletScriptSource m_bulletSource;
        private bool m_cachedDoDustups;
        private bool m_cachedGrounded;
        private Vector2 m_dashDirection;
        private float m_dashTimer;
        private bool m_shouldFire;
        private bool m_lastDashWasDouble;
        private DashBehavior.DashState m_state;

        public override void Start()
        {
            base.Start();
            this.m_trailRenderer = this.m_aiActor.GetComponentInChildren<TrailRenderer>();
            if (this.toggleTrailRenderer && (bool) (UnityEngine.Object) this.m_trailRenderer)
                this.m_trailRenderer.enabled = false;
            this.m_shadowTrail = this.m_aiActor.GetComponent<AfterImageTrailController>();
            if (this.bulletScript != null && !this.bulletScript.IsNull)
                this.m_aiActor.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventTriggered);
            if (!this.stopOnCollision)
                return;
            this.m_aiActor.specRigidbody.OnCollision += new Action<CollisionData>(this.OnCollision);
        }

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_dashTimer);
        }

        public override BehaviorResult Update()
        {
            if ((UnityEngine.Object) this.m_shadowSprite == (UnityEngine.Object) null)
                this.m_shadowSprite = this.m_aiActor.ShadowObject.GetComponent<tk2dBaseSprite>();
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
                return BehaviorResult.Continue;
            this.m_dashDirection = this.GetDashDirection();
            this.State = string.IsNullOrEmpty(this.chargeAnim) ? DashBehavior.DashState.Dash : DashBehavior.DashState.Charge;
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num = (int) base.ContinuousUpdate();
            if (this.State == DashBehavior.DashState.Charge)
            {
                if (!this.m_aiAnimator.IsPlaying(this.chargeAnim))
                    this.State = DashBehavior.DashState.Dash;
            }
            else if (this.State == DashBehavior.DashState.Dash)
            {
                if (this.doDodgeDustUp)
                {
                    bool flag = this.m_aiActor.spriteAnimator.QueryGroundedFrame();
                    if (!this.m_cachedGrounded && flag && !this.m_aiActor.IsFalling)
                    {
                        GameManager.Instance.Dungeon.dungeonDustups.InstantiateLandDustup((Vector3) this.m_aiActor.specRigidbody.UnitCenter);
                        this.m_aiActor.DoDustUps = this.m_cachedDoDustups;
                    }
                    this.m_cachedGrounded = flag;
                }
                if (this.enableShadowTrail && (double) this.m_dashTimer <= 0.10000000149011612)
                    this.m_shadowTrail.spawnShadows = false;
                if ((double) this.m_dashTimer <= 0.0)
                    return ContinuousBehaviorResult.Finished;
            }
            else if (this.State == DashBehavior.DashState.Idle)
                return ContinuousBehaviorResult.Finished;
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.m_updateEveryFrame = false;
            if ((double) this.postDashSpeed > 0.0)
            {
                this.m_aiActor.BehaviorVelocity = this.m_dashDirection.normalized * this.postDashSpeed;
            }
            else
            {
                this.m_aiActor.BehaviorOverridesVelocity = false;
                this.m_aiAnimator.LockFacingDirection = false;
            }
            this.State = DashBehavior.DashState.Idle;
            this.UpdateCooldowns();
            if (!this.m_lastDashWasDouble)
            {
                if ((double) this.doubleDashChance <= 0.0 || (double) UnityEngine.Random.value >= (double) this.doubleDashChance)
                    return;
                this.m_cooldownTimer = 0.0f;
                this.m_lastDashWasDouble = true;
            }
            else
                this.m_lastDashWasDouble = false;
        }

        public void AnimationEventTriggered(
            tk2dSpriteAnimator animator,
            tk2dSpriteAnimationClip clip,
            int frame)
        {
            if (this.m_state != DashBehavior.DashState.Dash || !this.m_shouldFire || !(clip.GetFrame(frame).eventInfo == "fire"))
                return;
            this.Fire();
        }

        public void OnCollision(CollisionData collisionData)
        {
            if (this.m_state != DashBehavior.DashState.Dash || collisionData.IsTriggerCollision || (bool) (UnityEngine.Object) collisionData.OtherRigidbody && (bool) (UnityEngine.Object) collisionData.OtherRigidbody.projectile)
                return;
            this.State = DashBehavior.DashState.Idle;
        }

        private float[] GetDirections()
        {
            float[] array = new float[0];
            if (this.dashDirection == DashBehavior.DashDirection.PerpendicularToTarget)
            {
                float angle = (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.Ground) - this.m_aiActor.specRigidbody.UnitCenter).ToAngle();
                array = new float[2]{ angle + 90f, angle - 90f };
                BraveUtility.RandomizeArray<float>(array);
            }
            else if (this.dashDirection == DashBehavior.DashDirection.KindaTowardTarget)
            {
                float angle = (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.Ground) - this.m_aiActor.specRigidbody.UnitCenter).ToAngle();
                array = new float[3]
                {
                    angle,
                    angle - this.quantizeDirection,
                    angle + this.quantizeDirection
                };
                BraveUtility.RandomizeArray<float>(array, 1);
            }
            else if (this.dashDirection == DashBehavior.DashDirection.TowardTarget)
                array = new float[1]
                {
                    (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.Ground) - this.m_aiActor.specRigidbody.UnitCenter).ToAngle()
                };
            else if (this.dashDirection == DashBehavior.DashDirection.Random)
            {
                if ((double) this.quantizeDirection <= 0.0)
                {
                    array = new float[16 /*0x10*/];
                    for (int index = 0; index < array.Length; ++index)
                        array[index] = UnityEngine.Random.Range(0.0f, 360f);
                }
                else
                {
                    array = new float[Mathf.RoundToInt(360f / this.quantizeDirection)];
                    for (int index = 0; index < array.Length; ++index)
                        array[index] = (float) index * this.quantizeDirection;
                    BraveUtility.RandomizeArray<float>(array);
                }
            }
            else if (this.dashDirection == DashBehavior.DashDirection.Random)
            {
                if ((double) this.quantizeDirection <= 0.0)
                {
                    array = new float[16 /*0x10*/];
                    for (int index = 0; index < array.Length; ++index)
                        array[index] = UnityEngine.Random.Range(0.0f, 360f);
                }
                else
                {
                    array = new float[Mathf.RoundToInt(360f / this.quantizeDirection)];
                    for (int index = 0; index < array.Length; ++index)
                        array[index] = (float) index * this.quantizeDirection;
                    BraveUtility.RandomizeArray<float>(array);
                }
            }
            if ((double) this.quantizeDirection > 0.0)
            {
                for (int index = 0; index < array.Length; ++index)
                    array[index] = BraveMathCollege.QuantizeFloat(array[index], this.quantizeDirection);
            }
            return array;
        }

        private Vector2 GetDashDirection()
        {
            float[] directions = this.GetDirections();
            Vector2 vector2_1 = Vector2.zero;
            Vector2 unitCenter1 = this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.Ground);
            for (int index = 0; index < directions.Length; ++index)
            {
                bool flag1 = false;
                bool flag2 = false;
                Vector2 vector1 = BraveMathCollege.DegreesToVector(directions[index]);
                RaycastResult result;
                bool flag3 = PhysicsEngine.Instance.Raycast(unitCenter1, vector1, this.dashDistance, out result, sourceLayer: new CollisionLayer?(CollisionLayer.EnemyCollider), ignoreRigidbody: this.m_aiActor.specRigidbody);
                RaycastResult.Pool.Free(ref result);
                for (float num = 0.25f; (double) num <= (double) this.dashDistance && !flag1 && !flag3; num += 0.25f)
                {
                    Vector2 vector2_2 = unitCenter1 + num * vector1;
                    if (!GameManager.Instance.Dungeon.CellExists(vector2_2))
                        flag1 = true;
                    else if (GameManager.Instance.Dungeon.ShouldReallyFall((Vector3) vector2_2))
                        flag1 = true;
                }
                for (float num = 0.25f; (double) num <= (double) this.dashDistance && !flag1 && !flag2 && !flag3; num += 0.25f)
                {
                    IntVector2 intVector2 = (unitCenter1 + num * vector1).ToIntVector2(VectorConversions.Floor);
                    if (!GameManager.Instance.Dungeon.CellExists(intVector2))
                        flag2 = true;
                    else if (GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2) && GameManager.Instance.Dungeon.data[intVector2].isExitCell)
                        flag2 = true;
                }
                if (this.avoidTarget && (bool) (UnityEngine.Object) this.m_behaviorSpeculator.TargetRigidbody && !flag1 && !flag2 && !flag3)
                {
                    Vector2 unitCenter2 = this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox);
                    Vector2 vector2 = this.m_behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - unitCenter2;
                    float num = this.dashDistance + 2f;
                    if ((double) vector2.magnitude < (double) num && (double) BraveMathCollege.AbsAngleBetween(vector2.ToAngle(), directions[index]) < 80.0)
                        flag3 = true;
                    if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && !flag3)
                    {
                        PlayerController playerTarget = this.m_aiActor.PlayerTarget as PlayerController;
                        if ((bool) (UnityEngine.Object) playerTarget)
                        {
                            PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(playerTarget);
                            if ((bool) (UnityEngine.Object) otherPlayer && otherPlayer.healthHaver.IsAlive)
                            {
                                vector2 = otherPlayer.specRigidbody.GetUnitCenter(ColliderType.HitBox) - unitCenter2;
                                if ((double) vector2.magnitude < (double) num && (double) BraveMathCollege.AbsAngleBetween(vector2.ToAngle(), directions[index]) < 80.0)
                                    flag3 = true;
                            }
                        }
                    }
                }
                if (!flag3 && !flag1 && !flag2)
                {
                    vector2_1 = vector1;
                    break;
                }
            }
            if (vector2_1 != Vector2.zero)
                return vector2_1.normalized;
            if (directions.Length > 0)
                return BraveMathCollege.DegreesToVector(directions[directions.Length - 1]);
            float num1 = UnityEngine.Random.Range(0.0f, 360f);
            if ((double) this.quantizeDirection > 0.0)
            {
                double num2 = (double) BraveMathCollege.QuantizeFloat(num1, this.quantizeDirection);
            }
            return BraveMathCollege.DegreesToVector(num1);
        }

        private void Fire()
        {
            if (!(bool) (UnityEngine.Object) this.m_bulletSource)
                this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
            this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
            this.m_bulletSource.BulletScript = this.bulletScript;
            this.m_bulletSource.Initialize();
            this.m_shouldFire = false;
        }

        private DashBehavior.DashState State
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

        private void BeginState(DashBehavior.DashState state)
        {
            switch (state)
            {
                case DashBehavior.DashState.Charge:
                    this.m_aiAnimator.LockFacingDirection = true;
                    this.m_aiAnimator.FacingDirection = this.m_dashDirection.ToAngle();
                    this.m_aiAnimator.PlayUntilFinished(this.chargeAnim, true);
                    this.m_aiActor.ClearPath();
                    if (this.m_aiActor.BehaviorOverridesVelocity)
                        break;
                    this.m_aiActor.BehaviorOverridesVelocity = true;
                    this.m_aiActor.BehaviorVelocity = Vector2.zero;
                    break;
                case DashBehavior.DashState.Dash:
                    if (this.bulletScript != null && !this.bulletScript.IsNull)
                        this.m_shouldFire = true;
                    this.m_aiAnimator.LockFacingDirection = true;
                    this.m_aiAnimator.FacingDirection = this.m_dashDirection.ToAngle();
                    if (!string.IsNullOrEmpty(this.dashAnim))
                    {
                        if (this.warpDashAnimLength)
                        {
                            AIAnimator aiAnimator = this.m_aiAnimator;
                            string dashAnim = this.dashAnim;
                            bool flag = true;
                            float dashTime = this.dashTime;
                            string name = dashAnim;
                            int num = flag ? 1 : 0;
                            double warpClipDuration = (double) dashTime;
                            aiAnimator.PlayUntilFinished(name, num != 0, warpClipDuration: (float) warpClipDuration);
                        }
                        else
                            this.m_aiAnimator.PlayUntilFinished(this.dashAnim, true);
                    }
                    if (this.doDodgeDustUp)
                    {
                        this.m_cachedDoDustups = this.m_aiActor.DoDustUps;
                        this.m_aiActor.DoDustUps = false;
                        GameManager.Instance.Dungeon.dungeonDustups.InstantiateDodgeDustup(this.m_dashDirection, (Vector3) this.m_aiActor.specRigidbody.UnitBottomCenter);
                        this.m_cachedGrounded = true;
                    }
                    if (this.hideShadow && (bool) (UnityEngine.Object) this.m_shadowSprite)
                        this.m_shadowSprite.renderer.enabled = false;
                    if (this.hideGun && (bool) (UnityEngine.Object) this.m_aiShooter)
                        this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (DashBehavior));
                    if (this.toggleTrailRenderer && (bool) (UnityEngine.Object) this.m_trailRenderer)
                        this.m_trailRenderer.enabled = true;
                    if (this.enableShadowTrail)
                    {
                        this.m_shadowTrail.spawnShadows = true;
                        int num = (int) AkSoundEngine.PostEvent("Play_ENM_highpriest_dash_01", GameManager.Instance.PrimaryPlayer.gameObject);
                    }
                    float num1 = this.dashDistance / this.dashTime;
                    this.m_dashTimer = this.dashTime;
                    this.m_aiActor.ClearPath();
                    this.m_aiActor.BehaviorOverridesVelocity = true;
                    this.m_aiActor.BehaviorVelocity = num1 * this.m_dashDirection;
                    if (this.bulletScript == null || this.bulletScript.IsNull || !this.fireAtDashStart)
                        break;
                    this.Fire();
                    break;
            }
        }

        private void EndState(DashBehavior.DashState state)
        {
            if (state != DashBehavior.DashState.Dash)
                return;
            if (!string.IsNullOrEmpty(this.dashAnim))
                this.m_aiAnimator.EndAnimationIf(this.dashAnim);
            if (this.bulletScript != null && !this.bulletScript.IsNull && this.m_shouldFire)
                this.Fire();
            if (this.doDodgeDustUp)
                this.m_aiActor.DoDustUps = this.m_cachedDoDustups;
            if (this.hideShadow && (bool) (UnityEngine.Object) this.m_shadowSprite)
                this.m_shadowSprite.renderer.enabled = true;
            if (this.hideGun && (bool) (UnityEngine.Object) this.m_aiShooter)
                this.m_aiShooter.ToggleGunAndHandRenderers(true, nameof (DashBehavior));
            if (this.toggleTrailRenderer && (bool) (UnityEngine.Object) this.m_trailRenderer)
                this.m_trailRenderer.enabled = false;
            if (this.enableShadowTrail)
                this.m_shadowTrail.spawnShadows = false;
            if ((double) this.postDashSpeed <= 0.0)
                this.m_aiActor.BehaviorVelocity = Vector2.zero;
            if (!((UnityEngine.Object) this.m_bulletSource != (UnityEngine.Object) null))
                return;
            this.m_bulletSource.ForceStop();
        }

        public enum DashDirection
        {
            PerpendicularToTarget = 10, // 0x0000000A
            KindaTowardTarget = 20, // 0x00000014
            TowardTarget = 25, // 0x00000019
            Random = 30, // 0x0000001E
        }

        private enum DashState
        {
            Idle,
            Charge,
            Dash,
        }
    }

