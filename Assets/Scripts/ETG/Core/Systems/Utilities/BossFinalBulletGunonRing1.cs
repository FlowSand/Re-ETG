using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalBullet/GunonRing1")]
public class BossFinalBulletGunonRing1 : Script
  {
    private const int NumBullets = 8;
    private const float FireRadius = 0.75f;
    private const float StartRadius = 4f;
    private const float EndRadius = 9f;
    private const int ExpandTime = 60;
    private const float SpinSpeed = 180f;
    private float SpinDirection;

    private float Radius { get; set; }

    private bool Done { get; set; }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossFinalBulletGunonRing1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class RingBullet : Bullet
    {
      private const float ReleaseSpinSpeed = 30f;
      private const float ReleaseDriftSpeed = 1f;
      private float m_angle;
      private int m_index;
      private BossFinalBulletGunonRing1 m_parentScript;

      public RingBullet(float angle, int index, BossFinalBulletGunonRing1 parentScript)
        : base()
      {
        this.m_angle = angle;
        this.m_index = index;
        this.m_parentScript = parentScript;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossFinalBulletGunonRing1.RingBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

    public class BatBullet : Bullet
    {
      public BatBullet(string name) : base(name)
      {
      }

      protected override IEnumerator Top()
      {
        this.Direction = this.GetAimDirection(this.Position, !BraveUtility.RandomBool() ? 1f : 0.0f, 12f);
        this.ChangeSpeed(new Brave.BulletScript.Speed(12f), 20);
        if (this.IsPointInTile(this.Position))
          this.Projectile.IgnoreTileCollisionsFor(1f);
        return (IEnumerator) null;
      }
    }
  }

