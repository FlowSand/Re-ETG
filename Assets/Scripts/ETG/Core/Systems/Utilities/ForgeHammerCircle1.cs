// Decompiled with JetBrains decompiler
// Type: ForgeHammerCircle1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

public class ForgeHammerCircle1 : Script
  {
    public int CircleBullets = 12;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ForgeHammerCircle1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class DefaultBullet : Bullet
    {
      public int spawnTime;

      public DefaultBullet(int spawnTime)
        : base()
      {
        this.spawnTime = spawnTime;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ForgeHammerCircle1.DefaultBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

