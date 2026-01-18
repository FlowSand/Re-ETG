// Decompiled with JetBrains decompiler
// Type: BulletKingSlam1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/BulletKing/Slam1")]
public class BulletKingSlam1 : Script
  {
    private const int NumBullets = 36;
    private const int NumHardBullets = 12;
    private const float RadiusAcceleration = 8f;
    private const float SpinAccelration = 10f;
    public static float SpinningBulletSpinSpeed = 180f;
    private const int Time = 180;

    protected bool IsHard => this is BulletKingSlamHard1;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletKingSlam1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class SpinningBullet : Bullet
    {
      private Vector2 centerPoint;
      private float startAngle;

      public SpinningBullet(Vector2 centerPoint, float startAngle, bool isHard)
        : base("slam", forceBlackBullet: isHard)
      {
        this.centerPoint = centerPoint;
        this.startAngle = startAngle;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletKingSlam1.SpinningBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

