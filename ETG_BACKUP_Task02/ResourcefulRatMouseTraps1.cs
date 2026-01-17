// Decompiled with JetBrains decompiler
// Type: ResourcefulRatMouseTraps1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/ResourcefulRat/MouseTraps1")]
public class ResourcefulRatMouseTraps1 : Script
{
  private const int FlightTime = 60;
  private const int NumTraps = 10;
  private static int[] s_xValues;
  private static int[] s_yValues;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ResourcefulRatMouseTraps1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void Fire(Vector2 goal)
  {
    float angle = (goal - this.Position).ToAngle();
    GameObject[] mouseTraps = this.BulletBank.GetComponent<ResourcefulRatController>().MouseTraps;
    this.Fire(new Brave.BulletScript.Direction(angle), (Bullet) new ResourcefulRatMouseTraps1.MouseTrapBullet(goal, BraveUtility.RandomElement<GameObject>(mouseTraps)));
  }

  private class MouseTrapBullet : Bullet
  {
    private Vector2 m_goalPos;
    private GameObject m_mouseTrapPrefab;

    public MouseTrapBullet(Vector2 goalPos, GameObject mouseTrapPrefab)
      : base("mousetrap", true)
    {
      this.m_goalPos = goalPos;
      this.m_mouseTrapPrefab = mouseTrapPrefab;
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ResourcefulRatMouseTraps1.MouseTrapBullet.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }
  }
}
