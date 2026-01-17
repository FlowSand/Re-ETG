// Decompiled with JetBrains decompiler
// Type: BossFinalRobotM16
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
    [InspectorDropdownName("Bosses/BossFinalRobot/M16")]
    public class BossFinalRobotM16 : Script
    {
      private const float NumBullets = 23f;
      private const int ArcTime = 54;
      private const float ShotVariance = 6f;
      private const float EllipseA = 2.92f;
      private const float EllipseB = 2.03f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossFinalRobotM16__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

}
