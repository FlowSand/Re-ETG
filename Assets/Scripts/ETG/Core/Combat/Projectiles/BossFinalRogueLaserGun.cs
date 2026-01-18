// Decompiled with JetBrains decompiler
// Type: BossFinalRogueLaserGun
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class BossFinalRogueLaserGun : BossFinalRogueGunController
  {
    [InspectorHeader("Beam Stuff")]
    public Transform beamTransform;
    public Projectile beamProjectile;
    public float fireTime = 6f;
    public bool doScreenShake;
    [InspectorShowIf("doScreenShake")]
    public ScreenShakeSettings screenShake;
    [InspectorHeader("Gun Motion")]
    public float sweepAngle;
    public float sweepAwayTime;
    public float sweepBackTime;
    public AdditionalBraveLight LightToTrigger;
    private bool m_firingLaser;
    private float m_laserAngle;
    private float m_fireTimer;
    private BasicBeamController m_laserBeam;

    public float LaserAngle
    {
      get => this.m_laserAngle;
      set
      {
        this.m_laserAngle = value;
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, this.m_laserAngle + 90f);
      }
    }

    public override void Start()
    {
      base.Start();
      this.ship.healthHaver.OnPreDeath += new Action<Vector2>(this.OnPreDeath);
    }

    public override void Update()
    {
      base.Update();
      if ((double) this.m_fireTimer <= 0.0)
        return;
      this.m_fireTimer -= BraveTime.DeltaTime;
      if ((double) this.m_fireTimer > 0.0)
        return;
      this.m_firingLaser = false;
    }

    public void LateUpdate()
    {
      if (this.m_firingLaser && (bool) (UnityEngine.Object) this.m_laserBeam)
      {
        this.m_laserBeam.LateUpdatePosition(this.beamTransform.position);
      }
      else
      {
        if (!(bool) (UnityEngine.Object) this.m_laserBeam || this.m_laserBeam.State != BasicBeamController.BeamState.Dissipating)
          return;
        this.m_laserBeam.LateUpdatePosition(this.beamTransform.position);
      }
    }

    protected override void OnDestroy() => base.OnDestroy();

    public override void Fire()
    {
      this.m_firingLaser = true;
      this.m_fireTimer = this.fireTime;
      this.LaserAngle = -90f;
      if ((bool) (UnityEngine.Object) this.LightToTrigger)
        this.LightToTrigger.ManuallyDoBulletSpawnedFade();
      this.StartCoroutine(this.FireBeam(this.beamProjectile));
      this.StartCoroutine(this.DoGunMotionCR());
      if (!this.doScreenShake)
        return;
      GameManager.Instance.MainCameraController.DoContinuousScreenShake(this.screenShake, (Component) this);
    }

    public override bool IsFinished => !this.m_firingLaser && !(bool) (UnityEngine.Object) this.m_laserBeam;

    public override void CeaseFire()
    {
      if ((bool) (UnityEngine.Object) this.LightToTrigger)
        this.LightToTrigger.EndEarly();
      this.m_firingLaser = false;
      if (!this.doScreenShake)
        return;
      GameManager.Instance.MainCameraController.StopContinuousScreenShake((Component) this);
    }

    public void OnPreDeath(Vector2 deathDir)
    {
      this.m_firingLaser = false;
      if (!(bool) (UnityEngine.Object) this.m_laserBeam)
        return;
      this.m_laserBeam.DestroyBeam();
      this.m_laserBeam = (BasicBeamController) null;
    }

    [DebuggerHidden]
    private IEnumerator DoGunMotionCR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossFinalRogueLaserGun__DoGunMotionCRc__Iterator0()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    protected IEnumerator FireBeam(Projectile projectile)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossFinalRogueLaserGun__FireBeamc__Iterator1()
      {
        projectile = projectile,
        _this = this
      };
    }
  }

