using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/BulletKing/HomingRing1")]
public class BulletKingHomingRing1 : Script
  {
    private const int NumBullets = 24;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletKingHomingRing1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class SmokeBullet : Bullet
    {
      private const float ExpandSpeed = 2f;
      private const float SpinSpeed = 120f;
      private const float Lifetime = 600f;
      private float m_angle;

      public SmokeBullet(float angle)
        : base("homingRing", forceBlackBullet: true)
      {
        this.m_angle = angle;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletKingHomingRing1.SmokeBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

