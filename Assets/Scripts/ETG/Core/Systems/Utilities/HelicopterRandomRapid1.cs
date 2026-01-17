// Decompiled with JetBrains decompiler
// Type: HelicopterRandomRapid1
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
    [InspectorDropdownName("Bosses/Helicopter/RandomBurstsRapid1")]
    public class HelicopterRandomRapid1 : Script
    {
      private const int NumBullets = 6;
      private static string[] Transforms = new string[4]
      {
        "shoot point 1",
        "shoot point 2",
        "shoot point 3",
        "shoot point 4"
      };

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HelicopterRandomRapid1.<Top>c__Iterator0()
        {
          $this = this
        };
      }
    }

}
