// Decompiled with JetBrains decompiler
// Type: InfiniteRunnerController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;

#nullable disable
public class InfiniteRunnerController : BraveBehaviour, IPlaceConfigurable
{
  [NonSerialized]
  public RoomHandler TargetRoom;

  public void ConfigureOnPlacement(RoomHandler room)
  {
    this.StartCoroutine(this.HandleDelayedInitialization(room));
  }

  [DebuggerHidden]
  private IEnumerator HandleDelayedInitialization(RoomHandler room)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new InfiniteRunnerController.\u003CHandleDelayedInitialization\u003Ec__Iterator0()
    {
      room = room,
      \u0024this = this
    };
  }

  public void StartQuest()
  {
    this.talkDoer.PathfindToPosition(this.TargetRoom.GetCenterCell().ToVector2());
  }

  protected override void OnDestroy() => base.OnDestroy();
}
