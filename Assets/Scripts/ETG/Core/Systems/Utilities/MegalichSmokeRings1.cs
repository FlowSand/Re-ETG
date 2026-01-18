// Decompiled with JetBrains decompiler
// Type: MegalichSmokeRings1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/Megalich/SmokeRings1")]
public class MegalichSmokeRings1 : Script
  {
    private const int NumRings = 4;
    private const int NumBullets = 24;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MegalichSmokeRings1__Topc__Iterator0()
      {
        _this = this
      };
    }

    public class SmokeBullet : Bullet
    {
      private const float ExpandSpeed = 4.5f;
      private const float SpinSpeed = 40f;
      private MegalichSmokeRings1 m_parent;
      private float m_angle;
      private float m_spinSpeed;

      public SmokeBullet(MegalichSmokeRings1 parent, float angle = 0.0f)
        : base("ring")
      {
        this.m_parent = parent;
        this.m_angle = angle;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MegalichSmokeRings1.SmokeBullet__Topc__Iterator0()
        {
          _this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator ChangeSpinSpeedTask(float newSpinSpeed, int term)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MegalichSmokeRings1.SmokeBullet__ChangeSpinSpeedTaskc__Iterator1()
        {
          newSpinSpeed = newSpinSpeed,
          term = term,
          _this = this
        };
      }
    }
  }

