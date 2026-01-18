// Decompiled with JetBrains decompiler
// Type: BulletKingHomingBurst1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/BulletKing/HomingBurst1")]
public class BulletKingHomingBurst1 : Script
  {
    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletKingHomingBurst1__Topc__Iterator0()
      {
        _this = this
      };
    }

    private void HomingShot(float x, float y, float direction)
    {
      this.Fire(new Offset(x, y, transform: string.Empty), new Brave.BulletScript.Direction(direction - 90f), new Brave.BulletScript.Speed(9f), (Bullet) new BulletKingHomingBurst1.HomingBullet());
    }

    public class HomingBullet : Bullet
    {
      public HomingBullet()
        : base("homing")
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletKingHomingBurst1.HomingBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

