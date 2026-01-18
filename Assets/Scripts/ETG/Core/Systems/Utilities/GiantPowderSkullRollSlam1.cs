// Decompiled with JetBrains decompiler
// Type: GiantPowderSkullRollSlam1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/GiantPowderSkull/RollSlam1")]
public class GiantPowderSkullRollSlam1 : Script
  {
    private const float OffsetDist = 1.5f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GiantPowderSkullRollSlam1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

