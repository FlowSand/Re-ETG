using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("BulletShotgunMan/ExecutionerDeathBurst1")]
public class BulletShotgunExecutionerManDeathBurst1 : Script
  {
    private const int NumInitialBullets = 6;
    private const int NumRingBullets = 12;
    private const float SpinSpeed = 540f;
    private const float FireRadius = 1f;
    private SpeculativeRigidbody m_targetRigidbody;
    private float? m_cachedRetargetAngle;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletShotgunExecutionerManDeathBurst1__Topc__Iterator0()
      {
        _this = this
      };
    }

    private float RetargetAngle
    {
      get
      {
        if ((bool) (Object) this.m_targetRigidbody)
          return (this.m_targetRigidbody.HitboxPixelCollider.UnitCenter - this.Position).ToAngle();
        if (!this.m_cachedRetargetAngle.HasValue)
          this.m_cachedRetargetAngle = new float?(Random.Range(0.0f, 360f));
        return this.m_cachedRetargetAngle.Value;
      }
    }

    public class RingBullet : Bullet
    {
      private float m_angle;
      private BulletShotgunExecutionerManDeathBurst1 m_parentScript;

      public RingBullet(
        float angle,
        BulletShotgunExecutionerManDeathBurst1 parentScript)
        : base("chain")
      {
        this.m_angle = angle;
        this.m_parentScript = parentScript;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletShotgunExecutionerManDeathBurst1.RingBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

