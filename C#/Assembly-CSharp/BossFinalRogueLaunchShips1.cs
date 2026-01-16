// Decompiled with JetBrains decompiler
// Type: BossFinalRogueLaunchShips1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public abstract class BossFinalRogueLaunchShips1 : Script
{
  private const int NumShipColumns = 4;
  private const int NumShipRows = 3;
  private const int LifeTime = 900;
  private const int MinShootInterval = 120;
  private const int MaxShootInterval = 240 /*0xF0*/;
  private Vector2 ShipPosMin = new Vector2(6f, -2f);
  private Vector2 ShipPosMax = new Vector2(16f, 2f);

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BossFinalRogueLaunchShips1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public class Ship : Bullet
  {
    private float m_desiredXOffset;
    private float m_desiredYOffset;
    private int m_spawnTime;

    public Ship(float xOffset, float yOffset, int spawnDelay)
      : base("anActualSpaceship")
    {
      this.m_desiredXOffset = xOffset;
      this.m_desiredYOffset = yOffset;
      this.m_spawnTime = spawnDelay;
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossFinalRogueLaunchShips1.Ship.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }
  }
}
