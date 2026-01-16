// Decompiled with JetBrains decompiler
// Type: AncientPistolController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
public class AncientPistolController : BraveBehaviour, IPlaceConfigurable
{
  [NonSerialized]
  public List<RoomHandler> RoomSequence;
  public List<bool> RoomSequenceEnemies;

  public void ConfigureOnPlacement(RoomHandler room)
  {
    this.StartCoroutine(this.HandleDelayedInitialization(room));
  }

  [DebuggerHidden]
  private IEnumerator HandleDelayedInitialization(RoomHandler room)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new AncientPistolController.\u003CHandleDelayedInitialization\u003Ec__Iterator0()
    {
      room = room,
      \u0024this = this
    };
  }

  protected override void OnDestroy() => base.OnDestroy();
}
