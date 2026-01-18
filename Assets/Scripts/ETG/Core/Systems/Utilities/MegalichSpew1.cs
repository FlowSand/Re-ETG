// Decompiled with JetBrains decompiler
// Type: MegalichSpew1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/Megalich/Spew1")]
public class MegalichSpew1 : Script
  {
    private const int NumInitialSnakes = 20;
    private const int NumLateSnakes = 20;
    private const int NumBullets = 5;
    private const int BulletSpeed = 12;
    private const float SnakeMagnitude = 0.75f;
    private const float SnakePeriod = 3f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MegalichSpew1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class SnakeBullet : Bullet
    {
      private int m_delay;
      private Vector2 m_target;
      private bool m_shouldHome;

      public SnakeBullet(int delay, Vector2 target, bool shouldHome)
        : base("spew")
      {
        this.m_delay = delay;
        this.m_target = target;
        this.m_shouldHome = shouldHome;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MegalichSpew1.SnakeBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

