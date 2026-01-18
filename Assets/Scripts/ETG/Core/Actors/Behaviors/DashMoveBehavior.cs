using System.Collections.Generic;

using FullInspector;
using UnityEngine;

#nullable disable

public class DashMoveBehavior : MovementBehaviorBase
    {
        [InspectorCategory("Conditions")]
        public float Cooldown = 1f;
        [InspectorCategory("Conditions")]
        public bool HealthModifiesCooldown;
        [InspectorCategory("Conditions")]
        [InspectorShowIf("HealthModifiesCooldown")]
        public float NoHealthCooldown = 1f;
        [InspectorShowIf("HealthModifiesCooldown")]
        [InspectorCategory("Conditions")]
        public float FullHealthCooldown = 1f;
        [InspectorCategory("Conditions")]
        public float InitialCooldown;
        [InspectorCategory("Conditions")]
        public float GlobalCooldown;
        public float dashDistance;
        public float dashTime;
        [InspectorCategory("Visuals")]
        public bool enableShadowTrail;
        protected float m_cooldownTimer;
        protected float m_dashTimer;

        public override void Start()
        {
            base.Start();
            this.m_cooldownTimer = this.InitialCooldown;
        }

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_cooldownTimer, true);
            this.DecrementTimer(ref this.m_dashTimer);
        }

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.IsReady() || !(bool) (Object) this.m_aiActor.TargetRigidbody)
                return BehaviorResult.Continue;
            float num1 = this.dashDistance / this.dashTime;
            this.m_aiActor.BehaviorOverridesVelocity = true;
            this.m_aiActor.BehaviorVelocity = num1 * this.GetDashDirection();
            this.m_updateEveryFrame = true;
            this.m_dashTimer = this.dashTime;
            if (this.enableShadowTrail)
                this.m_aiActor.GetComponent<AfterImageTrailController>().spawnShadows = true;
            int num2 = (int) AkSoundEngine.PostEvent("Play_ENM_highpriest_dash_01", GameManager.Instance.PrimaryPlayer.gameObject);
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num = (int) base.ContinuousUpdate();
            if (this.enableShadowTrail && (double) this.m_dashTimer <= 0.10000000149011612)
                this.m_aiActor.GetComponent<AfterImageTrailController>().spawnShadows = false;
            return (double) this.m_dashTimer <= 0.0 ? ContinuousBehaviorResult.Finished : ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.m_updateEveryFrame = false;
            this.m_aiActor.BehaviorOverridesVelocity = false;
            if (this.enableShadowTrail)
                this.m_aiActor.GetComponent<AfterImageTrailController>().spawnShadows = false;
            this.RefreshCooldowns();
        }

        public bool IsReady() => (double) this.m_cooldownTimer == 0.0;

        public void RefreshCooldowns()
        {
            this.m_cooldownTimer = !this.HealthModifiesCooldown ? this.Cooldown : Mathf.Lerp(this.NoHealthCooldown, this.FullHealthCooldown, this.m_aiActor.healthHaver.GetCurrentHealthPercentage());
            if ((double) this.GlobalCooldown <= 0.0)
                return;
            this.m_aiActor.behaviorSpeculator.GlobalCooldown = this.GlobalCooldown;
        }

        private Vector2 GetDashDirection()
        {
            Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
            Vector2 normalized = (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.Ground) - unitCenter).normalized;
            List<Vector2> list = new List<Vector2>();
            for (int index = 0; index < 2; ++index)
            {
                bool flag1 = false;
                bool flag2 = false;
                Vector2 direction = normalized.Rotate(index != 0 ? -90f : 90f);
                RaycastResult result;
                bool flag3 = PhysicsEngine.Instance.Raycast(unitCenter, direction, 3f, out result, sourceLayer: new CollisionLayer?(CollisionLayer.EnemyCollider), ignoreRigidbody: this.m_aiActor.specRigidbody);
                RaycastResult.Pool.Free(ref result);
                for (float num = 0.25f; (double) num <= (double) this.dashDistance && !flag1 && !flag3; num += 0.25f)
                {
                    Vector2 vector2 = unitCenter + num * direction;
                    if (!GameManager.Instance.Dungeon.CellExists(vector2))
                        flag1 = true;
                    else if (GameManager.Instance.Dungeon.ShouldReallyFall((Vector3) vector2))
                        flag1 = true;
                }
                for (float num = 0.25f; (double) num <= (double) this.dashDistance && !flag1 && !flag2 && !flag3; num += 0.25f)
                {
                    IntVector2 intVector2 = (unitCenter + num * direction).ToIntVector2(VectorConversions.Floor);
                    if (!GameManager.Instance.Dungeon.CellExists(intVector2))
                        flag2 = true;
                    else if (GameManager.Instance.Dungeon.data[intVector2].isExitCell)
                        flag2 = true;
                }
                if (!flag3 && !flag1 && !flag2)
                    list.Add(direction);
            }
            return list.Count > 0 ? BraveUtility.RandomElement<Vector2>(list).normalized : normalized.Rotate(BraveUtility.RandomSign() * 90f).normalized;
        }
    }

