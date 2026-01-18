// Decompiled with JetBrains decompiler
// Type: BulletKingDirectedFireLeft
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/BulletKing/DirectedFireLeft")]
public class BulletKingDirectedFireLeft : BulletKingDirectedFire
  {
    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletKingDirectedFireLeft__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

