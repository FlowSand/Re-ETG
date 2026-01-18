// Decompiled with JetBrains decompiler
// Type: HelicopterRandomSimple1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/Helicopter/RandomBurstsSimple1")]
public class HelicopterRandomSimple1 : Script
  {
    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new HelicopterRandomSimple1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class BigBullet : Bullet
    {
      public BigBullet()
        : base("big")
      {
      }

      protected override IEnumerator Top()
      {
        this.Projectile.Ramp(Random.Range(2f, 3f), 2f);
        return (IEnumerator) null;
      }
    }
  }

