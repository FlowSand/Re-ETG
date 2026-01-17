// Decompiled with JetBrains decompiler
// Type: MineFlayerBong1
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
    [InspectorDropdownName("Bosses/MineFlayer/Bong1")]
    public class MineFlayerBong1 : Script
    {
      private const int NumBullets = 90;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MineFlayerBong1__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

}
