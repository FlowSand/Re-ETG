// Decompiled with JetBrains decompiler
// Type: DraGunSpotlight1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/Spotlight1")]
public class DraGunSpotlight1 : Script
  {
    public const int ChaseTime = 480;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DraGunSpotlight1__Topc__Iterator0()
      {
        _this = this
      };
    }

    [DebuggerHidden]
    private IEnumerator UpdateSpotlightShrink()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DraGunSpotlight1__UpdateSpotlightShrinkc__Iterator1()
      {
        _this = this
      };
    }

    public override void OnForceEnded()
    {
      DraGunController component = this.BulletBank.GetComponent<DraGunController>();
      component.SpotlightEnabled = false;
      component.aiActor.ParentRoom.EndTerrifyingDarkRoom(0.5f);
      component.HandleDarkRoomEffects(false, 3f);
    }

    public class ArcBullet : Bullet
    {
      private Vector2 m_target;
      private float m_t;

      public ArcBullet(Vector2 target, float t)
        : base("triangle")
      {
        this.m_target = target;
        this.m_t = t;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new DraGunSpotlight1.ArcBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

