// Decompiled with JetBrains decompiler
// Type: ColorSquadHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
public class ColorSquadHelper : MonoBehaviour, IPlaceConfigurable
{
  private RoomHandler m_room;

  [DebuggerHidden]
  private IEnumerator Start()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new ColorSquadHelper.\u003CStart\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public void ConfigureOnPlacement(RoomHandler room) => this.m_room = room;
}
