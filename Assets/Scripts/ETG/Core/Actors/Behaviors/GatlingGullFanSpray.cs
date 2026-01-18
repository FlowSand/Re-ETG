using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/GatlingGull/FanSprayBehavior")]
public class GatlingGullFanSpray : BasicAttackBehavior
    {
        public float SprayAngle = 120f;
        public float SpraySpeed = 60f;
        public int SprayIterations = 4;
        public string OverrideBulletName;
        private float m_remainingDuration;
        private float m_totalDuration;

        public override void Start() => base.Start();

        public override void Upkeep() => base.Upkeep();

        public override BehaviorResult Update()
        {
            int num1 = (int) base.Update();
            if (!(bool) (Object) this.m_aiActor.TargetRigidbody)
                return BehaviorResult.Continue;
            this.m_aiShooter.volley.projectiles[0].angleVariance = 0.0f;
            int num2 = (int) AkSoundEngine.PostEvent("Play_ANM_Gull_Shoot_01", this.m_gameObject);
            this.m_totalDuration = this.SprayAngle / this.SpraySpeed * (float) this.SprayIterations;
            this.m_remainingDuration = this.m_totalDuration;
            this.m_aiActor.ClearPath();
            int num3 = (int) AkSoundEngine.PostEvent("Play_ANM_Gull_Loop_01", this.m_gameObject);
            int num4 = (int) AkSoundEngine.PostEvent("Play_ANM_Gull_Gatling_01", this.m_gameObject);
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num1 = (int) base.ContinuousUpdate();
            this.DecrementTimer(ref this.m_remainingDuration);
            if ((double) this.m_remainingDuration <= 0.0 || !(bool) (Object) this.m_aiActor.TargetRigidbody)
                return ContinuousBehaviorResult.Finished;
            float num2 = (float) ((1.0 - (double) this.m_remainingDuration / (double) this.m_totalDuration) * (double) this.SprayIterations % 2.0);
            this.m_aiShooter.ShootInDirection((Vector2) (Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.QuantizeFloat((this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiShooter.volleyShootPosition.position.XY()).ToAngle(), 45f) + ((float) (-(double) this.SprayAngle / 2.0) + Mathf.PingPong(num2 * this.SprayAngle, this.SprayAngle))) * (Vector3) Vector2.right), this.OverrideBulletName);
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            int num = (int) AkSoundEngine.PostEvent("Stop_ANM_Gull_Loop_01", this.m_gameObject);
            this.UpdateCooldowns();
        }

        public override void Destroy()
        {
            base.Destroy();
            if (!(bool) (Object) this.m_aiActor.GetComponent<AkGameObj>())
                return;
            int num = (int) AkSoundEngine.PostEvent("Stop_ANM_Gull_Loop_01", this.m_gameObject);
        }
    }

