// Decompiled with JetBrains decompiler
// Type: BossFinalRogueBulletScriptGun
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable

public class BossFinalRogueBulletScriptGun : BossFinalRogueGunController
  {
    public ShootBehavior ShootBehavior;
    private bool m_isRunning;

    public override void Start()
    {
      base.Start();
      this.ShootBehavior.Init(this.ship.gameObject, this.ship.aiActor, this.ship.aiShooter);
      this.ShootBehavior.Start();
    }

    public override void Update()
    {
      base.Update();
      this.ShootBehavior.SetDeltaTime(BraveTime.DeltaTime);
      this.ShootBehavior.Upkeep();
      if (!this.m_isRunning || this.ShootBehavior.ContinuousUpdate() != ContinuousBehaviorResult.Finished)
        return;
      this.m_isRunning = false;
      this.ShootBehavior.EndContinuousUpdate();
    }

    protected override void OnDestroy() => base.OnDestroy();

    public override void Fire()
    {
      switch (this.ShootBehavior.Update())
      {
        case BehaviorResult.RunContinuousInClass:
        case BehaviorResult.RunContinuous:
          this.m_isRunning = true;
          break;
      }
    }

    public override bool IsFinished => !this.m_isRunning;

    public override void CeaseFire()
    {
      if (!this.m_isRunning)
        return;
      this.ShootBehavior.EndContinuousUpdate();
    }
  }

