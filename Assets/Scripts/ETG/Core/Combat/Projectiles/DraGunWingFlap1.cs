using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/WingFlap1")]
public class DraGunWingFlap1 : Script
  {
    private const int NumBullets = 30;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DraGunWingFlap1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class WindProjectile : Bullet
    {
      private float m_sign;

      public WindProjectile(float sign)
        : base()
      {
        this.m_sign = sign;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DraGunWingFlap1.WindProjectile__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

