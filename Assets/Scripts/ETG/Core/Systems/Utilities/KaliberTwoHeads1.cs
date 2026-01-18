// Decompiled with JetBrains decompiler
// Type: KaliberTwoHeads1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Kaliber/TwoHeads1")]
public class KaliberTwoHeads1 : Script
  {
    private const int NumShots = 6;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new KaliberTwoHeads1__Topc__Iterator0()
      {
        _this = this
      };
    }

    private void FireArc(
      string transform,
      float startAngle,
      float sweepAngle,
      int numBullets,
      int muzzleIndex,
      bool offset)
    {
      float num = !offset ? (float) numBullets : (float) (numBullets - 1);
      for (int i = 0; (double) i < (double) num; ++i)
        this.Fire(new Offset(transform), new Brave.BulletScript.Direction(this.SubdivideArc(startAngle, sweepAngle, numBullets, i, offset)), new Brave.BulletScript.Speed(9f), new Bullet(suppressVfx: i != muzzleIndex));
    }
  }

