// Decompiled with JetBrains decompiler
// Type: AIBeamShooter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class AIBeamShooter : BraveBehaviour
    {
      public string shootAnim;
      public AIAnimator specifyAnimator;
      [Header("Beam Data")]
      public Transform beamTransform;
      public VFXPool chargeVfx;
      public Projectile beamProjectile;
      [HideInInspectorIf("beamProjectile", false)]
      public ProjectileModule beamModule;
      public bool TurnDuringDissipation = true;
      public bool PreventBeamContinuation;
      [Header("Depth")]
      public float heightOffset = 1.9f;
      public float northAngleTolerance = 90f;
      public float northRampHeight;
      public float otherRampHeight = 5f;
      [Header("Beam Firing Point")]
      public Vector2 firingEllipseCenter;
      public float firingEllipseA;
      public float firingEllipseB;
      public float eyeballFudgeAngle;
      private bool m_firingLaser;
      private float m_laserAngle;
      private BasicBeamController m_laserBeam;
      private BodyPartController m_bodyPart;

      public float LaserAngle
      {
        get => this.m_laserAngle;
        set
        {
          this.m_laserAngle = value;
          if (!this.m_firingLaser)
            return;
          this.CurrentAiAnimator.FacingDirection = value;
        }
      }

      public bool IsFiringLaser => this.m_firingLaser;

      public Vector2 LaserFiringCenter => this.beamTransform.position.XY() + this.firingEllipseCenter;

      public AIAnimator CurrentAiAnimator
      {
        get => (bool) (Object) this.specifyAnimator ? this.specifyAnimator : this.aiAnimator;
      }

      public float MaxBeamLength { get; set; }

      public BeamController LaserBeam => (BeamController) this.m_laserBeam;

      public bool IgnoreAiActorPlayerChecks { get; set; }

      public void Start()
      {
        this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnDamaged);
        if (!(bool) (Object) this.specifyAnimator)
          return;
        this.m_bodyPart = this.specifyAnimator.GetComponent<BodyPartController>();
      }

      public void Update()
      {
      }

      public void LateUpdate()
      {
        if ((bool) (Object) this.m_laserBeam && (double) this.MaxBeamLength > 0.0)
        {
          this.m_laserBeam.projectile.baseData.range = this.MaxBeamLength;
          this.m_laserBeam.ShowImpactOnMaxDistanceEnd = true;
        }
        if (this.m_firingLaser && (bool) (Object) this.m_laserBeam)
          this.m_laserBeam.LateUpdatePosition(this.GetTrueLaserOrigin());
        else if ((bool) (Object) this.m_laserBeam && this.m_laserBeam.State == BasicBeamController.BeamState.Dissipating)
        {
          this.m_laserBeam.LateUpdatePosition(this.GetTrueLaserOrigin());
        }
        else
        {
          if (!this.m_firingLaser || (bool) (Object) this.m_laserBeam)
            return;
          this.StopFiringLaser();
        }
      }

      protected override void OnDestroy()
      {
        if ((bool) (Object) this.m_laserBeam)
          this.m_laserBeam.CeaseAttack();
        base.OnDestroy();
      }

      public void OnDamaged(
        float resultValue,
        float maxValue,
        CoreDamageTypes damageTypes,
        DamageCategory damageCategory,
        Vector2 damageDirection)
      {
        if ((double) resultValue > 0.0)
          return;
        if (this.m_firingLaser)
        {
          this.chargeVfx.DestroyAll();
          this.StopFiringLaser();
        }
        if (!(bool) (Object) this.m_laserBeam)
          return;
        this.m_laserBeam.DestroyBeam();
        this.m_laserBeam = (BasicBeamController) null;
      }

      public void StartFiringLaser(float laserAngle)
      {
        this.m_firingLaser = true;
        this.LaserAngle = laserAngle;
        if ((bool) (Object) this.m_bodyPart)
          this.m_bodyPart.OverrideFacingDirection = true;
        if (!string.IsNullOrEmpty(this.shootAnim))
        {
          this.CurrentAiAnimator.LockFacingDirection = true;
          this.CurrentAiAnimator.PlayUntilCancelled(this.shootAnim, true);
        }
        this.chargeVfx.DestroyAll();
        this.StartCoroutine(this.FireBeam(!(bool) (Object) this.beamProjectile ? this.beamModule.GetCurrentProjectile() : this.beamProjectile));
      }

      public void StopFiringLaser()
      {
        this.m_firingLaser = false;
        if ((bool) (Object) this.m_bodyPart)
          this.m_bodyPart.OverrideFacingDirection = false;
        if (string.IsNullOrEmpty(this.shootAnim))
          return;
        this.CurrentAiAnimator.LockFacingDirection = false;
        this.CurrentAiAnimator.EndAnimationIf(this.shootAnim);
      }

      [DebuggerHidden]
      protected IEnumerator FireBeam(Projectile projectile)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AIBeamShooter.<FireBeam>c__Iterator0()
        {
          projectile = projectile,
          _this = this
        };
      }

      private Vector3 GetTrueLaserOrigin()
      {
        Vector2 center = this.LaserFiringCenter;
        if ((double) this.firingEllipseA != 0.0 && (double) this.firingEllipseB != 0.0)
        {
          float num = Mathf.Lerp(this.eyeballFudgeAngle, 0.0f, BraveMathCollege.AbsAngleBetween(90f, Mathf.Abs(BraveMathCollege.ClampAngle180(this.LaserAngle))) / 90f);
          center = BraveMathCollege.GetEllipsePoint(center, this.firingEllipseA, this.firingEllipseB, this.LaserAngle + num);
        }
        return (Vector3) center;
      }
    }

}
