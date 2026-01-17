// Decompiled with JetBrains decompiler
// Type: TankTreaderMiniTurretController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class TankTreaderMiniTurretController : BodyPartController
{
  public GameObject ShootPoint;
  public string BulletName;
  public float FireTime;
  public float ShotCooldown;
  public float MinCooldown;
  public float MaxCooldown;
  public float StartAngle;
  public float SweepAngle;
  private float m_fireTimeRemaining;
  private float m_timeUntilNextShot;
  private float m_cooldown;
  private static int m_arcCount;
  private static int m_lastFrame;
  private TankTreaderMiniTurretController.State m_state;

  public TankTreaderMiniTurretController.AimMode aimMode { get; set; }

  public float? OverrideAngle { get; set; }

  public override void Start()
  {
    base.Start();
    this.aimMode = TankTreaderMiniTurretController.AimMode.Away;
  }

  public void OnEnable()
  {
    this.m_state = TankTreaderMiniTurretController.State.Cooldown;
    this.m_cooldown = Random.Range(this.MinCooldown, this.MaxCooldown);
  }

  public override void Update()
  {
    base.Update();
    bool flag = false;
    if (this.aimMode == TankTreaderMiniTurretController.AimMode.AtPlayer)
    {
      if ((bool) (Object) this.m_body.TargetRigidbody)
        flag = BraveMathCollege.IsAngleWithinSweepArea((this.m_body.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - (Vector2) this.transform.position).ToAngle(), (float) ((double) this.StartAngle + (double) this.m_body.aiAnimator.FacingDirection + 90.0), this.SweepAngle);
    }
    else if (this.aimMode == TankTreaderMiniTurretController.AimMode.Away)
      flag = true;
    if (this.m_state == TankTreaderMiniTurretController.State.Idle)
    {
      if (!flag)
        return;
      this.m_state = TankTreaderMiniTurretController.State.Firing;
      this.m_fireTimeRemaining = this.FireTime;
      this.m_timeUntilNextShot = 0.0f;
    }
    else if (this.m_state == TankTreaderMiniTurretController.State.Firing)
    {
      this.m_fireTimeRemaining -= BraveTime.DeltaTime;
      this.m_timeUntilNextShot -= BraveTime.DeltaTime;
      if (!flag || (double) this.m_fireTimeRemaining <= 0.0)
      {
        this.m_state = TankTreaderMiniTurretController.State.Cooldown;
        this.m_cooldown = Random.Range(this.MinCooldown, this.MaxCooldown);
      }
      else
      {
        if ((double) this.m_timeUntilNextShot > 0.0)
          return;
        this.Fire();
        this.m_timeUntilNextShot = this.ShotCooldown;
      }
    }
    else
    {
      if (this.m_state != TankTreaderMiniTurretController.State.Cooldown)
        return;
      this.m_cooldown = Mathf.Max(0.0f, this.m_cooldown - BraveTime.DeltaTime);
      if ((double) this.m_cooldown > 0.0)
        return;
      this.m_state = TankTreaderMiniTurretController.State.Idle;
    }
  }

  protected override void OnDestroy() => base.OnDestroy();

  private void Fire()
  {
    Projectile component = this.m_body.bulletBank.CreateProjectileFromBank((Vector2) this.ShootPoint.transform.position, this.transform.eulerAngles.z, this.BulletName).GetComponent<Projectile>();
    Vector2 vector = BraveMathCollege.DegreesToVector(this.transform.eulerAngles.z, component.baseData.speed);
    Vector2 velocity = this.specifyActor.specRigidbody.Velocity;
    component.Direction = (vector + velocity).normalized;
    component.Speed = (vector + velocity).magnitude;
  }

  protected override bool TryGetAimAngle(out float angle)
  {
    if (this.OverrideAngle.HasValue)
    {
      angle = this.OverrideAngle.Value;
      return true;
    }
    if (this.aimMode != TankTreaderMiniTurretController.AimMode.Away)
      return base.TryGetAimAngle(out angle);
    angle = this.StartAngle + 0.5f * this.SweepAngle;
    angle += this.m_body.aiAnimator.FacingDirection + 90f;
    return true;
  }

  public enum AimMode
  {
    AtPlayer,
    Away,
  }

  private enum State
  {
    Idle,
    Firing,
    Cooldown,
  }
}
