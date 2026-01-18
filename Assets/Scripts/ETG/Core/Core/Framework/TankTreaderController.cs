using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class TankTreaderController : BraveBehaviour
  {
    public GameObject mainGun;
    public float backTurretOffset = 30f;
    public tk2dSpriteAnimator guy;
    public tk2dSpriteAnimator hatch;
    public GameObject hatchPopObject;
    public VFXPool hatchPopVfx;
    public float hatchFlyTime = 1f;
    public Vector2 hatchFlySpeed = (Vector2) new Vector3(8f, 20f);
    private TankTreaderMiniTurretController[] m_miniTurrets;
    private ParticleSystem[] m_exhaustParticleSystems;
    private int m_exhaustFrameCount;
    private bool m_hasPoppedHatch;

    public void Start()
    {
      this.m_miniTurrets = this.GetComponentsInChildren<TankTreaderMiniTurretController>();
      this.m_exhaustParticleSystems = this.GetComponentsInChildren<ParticleSystem>();
      this.aiActor.OverrideHitEnemies = true;
      this.guy.OnPlayAnimationCalled += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnGuyAnimation);
      this.healthHaver.bodySprites.Add(this.hatch.sprite);
      this.healthHaver.bodySprites.Add(this.guy.sprite);
    }

    public void Update()
    {
      if ((bool) (UnityEngine.Object) this.aiActor.TargetRigidbody)
      {
        Vector2 unitCenter = this.aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
        float num1 = Vector2.Distance((Vector2) this.mainGun.transform.position, unitCenter);
        float angle = (this.mainGun.transform.position.XY() - unitCenter).ToAngle();
        for (int index = 0; index < this.m_miniTurrets.Length; ++index)
        {
          TankTreaderMiniTurretController miniTurret = this.m_miniTurrets[index];
          if ((double) Vector2.Distance((Vector2) miniTurret.transform.position, unitCenter) < (double) num1)
          {
            miniTurret.aimMode = TankTreaderMiniTurretController.AimMode.AtPlayer;
            miniTurret.OverrideAngle = new float?();
          }
          else
          {
            miniTurret.aimMode = TankTreaderMiniTurretController.AimMode.Away;
            float num2 = (double) BraveMathCollege.ClampAngle180((miniTurret.transform.position.XY() - unitCenter).ToAngle() - angle) >= 0.0 ? -1f : 1f;
            miniTurret.OverrideAngle = new float?((unitCenter - miniTurret.transform.position.XY()).ToAngle() + num2 * this.backTurretOffset);
          }
        }
      }
      bool enabled = true;
      if (this.aiActor.BehaviorVelocity != Vector2.zero && (double) BraveMathCollege.AbsAngleBetween(this.aiActor.BehaviorVelocity.ToAngle(), this.aiAnimator.FacingDirection) > 170.0)
        enabled = false;
      if (enabled)
      {
        if (this.m_exhaustFrameCount++ < 5)
          enabled = false;
      }
      else
        this.m_exhaustFrameCount = 0;
      for (int index = 0; index < this.m_exhaustParticleSystems.Length; ++index)
        BraveUtility.EnableEmission(this.m_exhaustParticleSystems[index], enabled);
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      int num = (int) AkSoundEngine.PostEvent("Stop_BOSS_tank_idle_01", this.gameObject);
    }

    private void OnGuyAnimation(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
    {
      if (this.m_hasPoppedHatch || !(clip.name == "guy_in_gun") && !clip.name.StartsWith("guy_fire"))
        return;
      this.StartCoroutine(this.PopHatchCR());
      this.m_hasPoppedHatch = true;
    }

    [DebuggerHidden]
    private IEnumerator PopHatchCR()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TankTreaderController__PopHatchCRc__Iterator0()
      {
        _this = this
      };
    }
  }

