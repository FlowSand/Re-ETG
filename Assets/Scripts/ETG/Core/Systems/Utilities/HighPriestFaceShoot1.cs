// Decompiled with JetBrains decompiler
// Type: HighPriestFaceShoot1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/HighPriest/FaceShoot1")]
public class HighPriestFaceShoot1 : Script
  {
    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new HighPriestFaceShoot1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class FastHomingShot : Bullet
    {
      public FastHomingShot()
        : base("quickHoming")
      {
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HighPriestFaceShoot1.FastHomingShot__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

