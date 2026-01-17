// Decompiled with JetBrains decompiler
// Type: DemonWallSpew1
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
    [InspectorDropdownName("Bosses/DemonWall/Spew1")]
    public class DemonWallSpew1 : Script
    {
      public static string[][] shootPoints = new string[2][]
      {
        new string[3]{ "sad bullet", "blobulon", "dopey bullet" },
        new string[4]
        {
          "sideways bullet",
          "shotgun bullet",
          "cultist",
          "angry bullet"
        }
      };
      private const int NumBullets = 12;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DemonWallSpew1__Topc__Iterator0()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator FireWall(float sign)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DemonWallSpew1__FireWallc__Iterator1()
        {
          sign = sign,
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator FireWaves(int index)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DemonWallSpew1__FireWavesc__Iterator2()
        {
          index = index,
          _this = this
        };
      }
    }

}
