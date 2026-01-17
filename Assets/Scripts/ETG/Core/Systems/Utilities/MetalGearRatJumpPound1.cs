// Decompiled with JetBrains decompiler
// Type: MetalGearRatJumpPound1
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
    [InspectorDropdownName("Bosses/MetalGearRat/JumpPound1")]
    public class MetalGearRatJumpPound1 : Script
    {
      private const int NumWaves = 3;
      private const int NumBullets = 43;
      private const float EllipseA = 6f;
      private const float EllipseB = 2f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MetalGearRatJumpPound1.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }
    }

}
