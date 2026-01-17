// Decompiled with JetBrains decompiler
// Type: BossFinalRobotGrenades1
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
    [InspectorDropdownName("Bosses/BossFinalRobot/Grenades1")]
    public class BossFinalRobotGrenades1 : Script
    {
      private const int NumBullets = 4;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossFinalRobotGrenades1__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

}
