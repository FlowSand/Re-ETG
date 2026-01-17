// Decompiled with JetBrains decompiler
// Type: GiantPowderSkullEyesTwinBeams1
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
    [InspectorDropdownName("Bosses/GiantPowderSkull/EyesTwinBeams1")]
    public class GiantPowderSkullEyesTwinBeams1 : Script
    {
      private const float CoreSpread = 20f;
      private const float IncSpread = 10f;
      private const float TurnSpeed = 1f;
      private const float BulletSpeed = 18f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new GiantPowderSkullEyesTwinBeams1__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

}
