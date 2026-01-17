// Decompiled with JetBrains decompiler
// Type: HighPriestMergoWave1
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
    [InspectorDropdownName("Bosses/HighPriest/MergoWave1")]
    public class HighPriestMergoWave1 : Script
    {
      private const int NumBullets = 15;
      private const float Angle = 120f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HighPriestMergoWave1__Topc__Iterator0()
        {
          _this = this
        };
      }
    }

}
