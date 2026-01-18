using System;

using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/BossStatues/CrosshairBehavior")]
public class BossStatuesCrosshairBehavior : BossStatuesPatternBehavior
    {
        public float CircleRadius;
        public float InitialJumpDelay;
        public float SequentialJumpDelays;
        public string AttackVfx;
        public float AttackVfxPreTimer;
        public BulletScriptSelector BulletScript;
        private BulletScriptSource m_bulletSource;
        private float[] m_statueAngles;
        private float m_cachedStatueAngle;
        private float m_jumpTimer;
        private bool m_hasStarted;
        private bool m_isGrounded;
        private bool m_hasPlayedAttackVfx;

        public override void Start()
        {
            base.Start();
            this.m_cachedStatueAngle = (float) (0.5 * (360.0 / (double) this.m_statuesController.allStatues.Count));
            if (!TurboModeController.IsActive)
                return;
            this.InitialJumpDelay /= TurboModeController.sEnemyBulletSpeedMultiplier;
            this.SequentialJumpDelays /= TurboModeController.sEnemyBulletSpeedMultiplier;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            if (this.m_state == BossStatuesPatternBehavior.PatternState.InProgress)
            {
                if (!this.m_hasStarted)
                {
                    if (!this.m_hasPlayedAttackVfx && (double) (this.m_statuesController.attackHopTime - this.m_timeElapsed) < (double) this.AttackVfxPreTimer)
                    {
                        this.m_statuesController.GetComponent<VfxController>().PlayVfx(this.AttackVfx);
                        this.m_hasPlayedAttackVfx = true;
                    }
                    if ((double) this.m_timeElapsed > 0.10000000149011612)
                    {
                        this.SetActiveState(BossStatueController.StatueState.StandStill);
                        if (this.AreAllGrounded())
                        {
                            this.m_hasStarted = true;
                            this.ShootBulletScript();
                            this.m_hasPlayedAttackVfx = false;
                            this.m_isGrounded = true;
                            this.m_jumpTimer = this.InitialJumpDelay;
                        }
                    }
                }
                else
                {
                    this.m_jumpTimer -= this.m_deltaTime;
                    if (!this.m_hasPlayedAttackVfx && (double) this.m_jumpTimer < (double) this.AttackVfxPreTimer)
                    {
                        this.m_statuesController.GetComponent<VfxController>().PlayVfx(this.AttackVfx);
                        this.m_hasPlayedAttackVfx = true;
                    }
                    if (this.m_isGrounded)
                    {
                        if ((double) this.m_jumpTimer <= (double) this.m_statuesController.attackHopTime)
                        {
                            float val2 = -1f;
                            for (int index = 0; index < this.m_activeStatueCount; ++index)
                            {
                                if ((bool) (UnityEngine.Object) this.m_activeStatues[index] && this.m_activeStatues[index].healthHaver.IsAlive)
                                {
                                    this.m_activeStatues[index].QueuedBulletScript.Add((BulletScriptSelector) null);
                                    this.m_activeStatues[index].State = BossStatueController.StatueState.HopToTarget;
                                    val2 = Math.Max(this.m_activeStatues[index].DistancetoTarget, val2);
                                }
                            }
                            if ((double) val2 > 0.0)
                                this.m_statuesController.OverrideMoveSpeed = new float?(Mathf.Max(this.m_statuesController.moveSpeed, 1.5f * val2 / this.m_statuesController.attackHopTime));
                            this.m_jumpTimer += this.SequentialJumpDelays;
                            this.m_isGrounded = false;
                        }
                    }
                    else
                    {
                        this.m_isGrounded = true;
                        for (int index = 0; index < this.m_activeStatueCount; ++index)
                        {
                            if ((bool) (UnityEngine.Object) this.m_activeStatues[index] && this.m_activeStatues[index].healthHaver.IsAlive)
                            {
                                this.m_activeStatues[index].State = BossStatueController.StatueState.StandStill;
                                this.m_isGrounded &= this.m_activeStatues[index].IsGrounded;
                            }
                        }
                        if (this.m_isGrounded)
                        {
                            this.m_hasPlayedAttackVfx = false;
                            this.m_statuesController.OverrideMoveSpeed = new float?();
                        }
                    }
                }
            }
            return base.ContinuousUpdate();
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.m_cachedStatueAngle = BraveMathCollege.ClampAngle360(this.m_statueAngles[0]);
        }

        protected override void InitPositions()
        {
            this.m_statueAngles = new float[this.m_activeStatueCount];
            for (int index = 0; index < this.m_activeStatueCount; ++index)
                this.m_statueAngles[index] = this.m_cachedStatueAngle + (float) index * (360f / (float) this.m_activeStatueCount);
            Vector2[] positions = new Vector2[this.m_activeStatueCount];
            for (int index = 0; index < this.m_activeStatueCount; ++index)
                positions[index] = this.GetTargetPoint(this.m_statueAngles[index]);
            this.ReorderStatues(positions);
            for (int index = 0; index < positions.Length; ++index)
                this.m_activeStatues[index].Target = new Vector2?(this.GetTargetPoint(this.m_statueAngles[index]));
            this.m_hasStarted = false;
        }

        protected override void UpdatePositions()
        {
        }

        protected override bool IsFinished() => this.m_hasStarted && this.m_bulletSource.IsEnded;

        protected override void OnStatueDeath()
        {
            if (!(bool) (UnityEngine.Object) this.m_bulletSource)
                return;
            this.m_statuesController.ClearBullets((Vector2) this.m_bulletSource.transform.position);
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_bulletSource);
            this.m_bulletSource = (BulletScriptSource) null;
            int num = (int) AkSoundEngine.PostEvent("Stop_ENM_statue_ring_01", this.m_statuesController.bulletBank.gameObject);
        }

        protected override void BeginState(BossStatuesPatternBehavior.PatternState state)
        {
            base.BeginState(state);
            if (state != BossStatuesPatternBehavior.PatternState.InProgress)
                return;
            this.m_hasStarted = false;
            this.m_hasPlayedAttackVfx = false;
            for (int index = 0; index < this.m_activeStatueCount; ++index)
            {
                BossStatueController activeStatue = this.m_activeStatues[index];
                if ((bool) (UnityEngine.Object) activeStatue && activeStatue.healthHaver.IsAlive)
                {
                    activeStatue.knockbackDoer.SetImmobile(true, "CrosshairBehavior");
                    activeStatue.healthHaver.AllDamageMultiplier *= 0.5f;
                    activeStatue.QueuedBulletScript.Add((BulletScriptSelector) null);
                    activeStatue.State = BossStatueController.StatueState.HopToTarget;
                    activeStatue.SuppressShootVfx = true;
                }
            }
        }

        protected override void EndState(BossStatuesPatternBehavior.PatternState state)
        {
            switch (state)
            {
                case BossStatuesPatternBehavior.PatternState.MovingToStartingPosition:
                    this.m_statuesController.IsTransitioning = false;
                    break;
                case BossStatuesPatternBehavior.PatternState.InProgress:
                    if ((double) this.OverrideMoveSpeed > 0.0)
                        this.m_statuesController.OverrideMoveSpeed = new float?();
                    for (int index = 0; index < this.m_activeStatueCount; ++index)
                    {
                        BossStatueController activeStatue = this.m_activeStatues[index];
                        if ((bool) (UnityEngine.Object) activeStatue && activeStatue.healthHaver.IsAlive)
                        {
                            activeStatue.knockbackDoer.SetImmobile(false, "CrosshairBehavior");
                            activeStatue.healthHaver.AllDamageMultiplier *= 2f;
                            activeStatue.SuppressShootVfx = true;
                        }
                    }
                    break;
            }
        }

        private Vector2 GetTargetPoint(float angle)
        {
            return this.m_statuesController.PatternCenter + BraveMathCollege.DegreesToVector(angle, this.CircleRadius);
        }

        private void ShootBulletScript()
        {
            if (!(bool) (UnityEngine.Object) this.m_bulletSource)
            {
                Transform transform = new GameObject("crazy shoot point").transform;
                transform.position = (Vector3) this.m_statuesController.PatternCenter;
                this.m_bulletSource = transform.gameObject.GetOrAddComponent<BulletScriptSource>();
            }
            this.m_bulletSource.BulletManager = this.m_statuesController.bulletBank;
            this.m_bulletSource.BulletScript = this.BulletScript;
            this.m_bulletSource.Initialize();
        }
    }

