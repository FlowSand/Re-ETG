using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/MineFlayer/Shoot1")]
public class MineFlayerShoot1 : Script
  {
    private const int NumBursts = 5;
    private const int NumBullets = 36;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MineFlayerShoot1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class BurstBullet : Bullet
    {
      private Vector2 m_addtionalVelocity;

      public BurstBullet(Vector2 additionalVelocity)
        : base("burst")
      {
        this.m_addtionalVelocity = additionalVelocity;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MineFlayerShoot1.BurstBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

