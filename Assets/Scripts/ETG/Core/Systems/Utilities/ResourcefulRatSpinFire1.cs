// Decompiled with JetBrains decompiler
// Type: ResourcefulRatSpinFire1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/ResourcefulRat/SpinFire1")]
    public class ResourcefulRatSpinFire1 : Script
    {
      private const float NumBullets = 23f;
      private const int ArcTime = 70;
      private const float SpreadAngle = 6f;
      private const float BulletSpeed = 16f;
      private const float EllipseA = 1.39f;
      private const float EllipseB = 0.92f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ResourcefulRatSpinFire1__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

}
