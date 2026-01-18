using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/GatlingGull/WalkAndShoot")]
public class GatlingGullWalkAndShoot : BasicAttackBehavior
  {
    public float Duration = 5f;
    public float AngleVariance = 20f;
    public bool ContinuesOnPathComplete;
    public string OverrideBulletName;
    private float m_durationTimer;

    public override void Start() => base.Start();

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_durationTimer);
    }

    public override BehaviorResult Update()
    {
      int num1 = (int) base.Update();
      if (!(bool) (Object) this.m_aiActor.TargetRigidbody)
        return BehaviorResult.Continue;
      this.m_durationTimer = this.Duration;
      this.m_aiActor.SuppressTargetSwitch = true;
      int num2 = (int) AkSoundEngine.PostEvent("Play_ANM_Gull_Loop_01", this.m_gameObject);
      int num3 = (int) AkSoundEngine.PostEvent("Play_ANM_Gull_Gatling_01", this.m_gameObject);
      return BehaviorResult.RunContinuousInClass;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      if (this.ContinuesOnPathComplete)
        this.m_aiAnimator.OverrideIdleAnimation = "idle_shoot";
      if ((double) this.m_durationTimer <= 0.0 || !(bool) (Object) this.m_aiActor.TargetRigidbody || this.m_aiActor.PathComplete && !this.ContinuesOnPathComplete)
        return ContinuousBehaviorResult.Finished;
      Vector2 inVec = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiActor.CenterPosition;
      int octant = BraveMathCollege.VectorToOctant(inVec);
      this.m_aiShooter.ManualGunAngle = true;
      this.m_aiShooter.GunAngle = Mathf.Atan2(inVec.y, inVec.x) * 57.29578f;
      Vector2 direction = (Vector2) (Quaternion.Euler(0.0f, 0.0f, (float) (octant * -45)) * (Vector3) Vector2.up);
      this.m_aiShooter.volley.projectiles[0].angleVariance = this.AngleVariance;
      this.m_aiShooter.ShootInDirection(direction, this.OverrideBulletName);
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      if (this.ContinuesOnPathComplete)
        this.m_aiAnimator.OverrideIdleAnimation = string.Empty;
      this.m_aiShooter.ManualGunAngle = false;
      this.UpdateCooldowns();
      this.m_aiActor.SuppressTargetSwitch = false;
      int num = (int) AkSoundEngine.PostEvent("Stop_ANM_Gull_Loop_01", this.m_gameObject);
    }

    public override void Destroy()
    {
      base.Destroy();
      if (!(bool) (Object) this.m_aiActor.GetComponent<AkGameObj>())
        return;
      int num = (int) AkSoundEngine.PostEvent("Stop_ANM_Gull_Loop_01", this.m_gameObject);
    }
  }

