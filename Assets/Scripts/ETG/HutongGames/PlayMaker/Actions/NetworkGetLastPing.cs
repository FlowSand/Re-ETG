// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkGetLastPing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Network)]
  [HutongGames.PlayMaker.Tooltip("Get the last ping time to the given player in milliseconds. \nIf the player can't be found -1 will be returned. Pings are automatically sent out every couple of seconds.")]
  public class NetworkGetLastPing : FsmStateAction
  {
    [ActionSection("Setup")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The Index of the player in the network connections list.")]
    public FsmInt playerIndex;
    [HutongGames.PlayMaker.Tooltip("The player reference is cached, that is if the connections list changes, the player reference remains.")]
    public bool cachePlayerReference = true;
    public bool everyFrame;
    [HutongGames.PlayMaker.Tooltip("Get the last ping time to the given player in milliseconds.")]
    [RequiredField]
    [ActionSection("Result")]
    [UIHint(UIHint.Variable)]
    public FsmInt lastPing;
    [HutongGames.PlayMaker.Tooltip("Event to send if the player can't be found. Average Ping is set to -1.")]
    public FsmEvent PlayerNotFoundEvent;
    [HutongGames.PlayMaker.Tooltip("Event to send if the player is found (pings back).")]
    public FsmEvent PlayerFoundEvent;
    private NetworkPlayer _player;

    public override void Reset()
    {
      this.playerIndex = (FsmInt) null;
      this.lastPing = (FsmInt) null;
      this.PlayerNotFoundEvent = (FsmEvent) null;
      this.PlayerFoundEvent = (FsmEvent) null;
      this.cachePlayerReference = true;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      if (this.cachePlayerReference)
        this._player = Network.connections[this.playerIndex.Value];
      this.GetLastPing();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.GetLastPing();

    private void GetLastPing()
    {
      if (!this.cachePlayerReference)
        this._player = Network.connections[this.playerIndex.Value];
      int lastPing = Network.GetLastPing(this._player);
      this.lastPing.Value = lastPing;
      if (lastPing == -1 && this.PlayerNotFoundEvent != null)
        this.Fsm.Event(this.PlayerNotFoundEvent);
      if (lastPing == -1 || this.PlayerFoundEvent == null)
        return;
      this.Fsm.Event(this.PlayerFoundEvent);
    }
  }
}
