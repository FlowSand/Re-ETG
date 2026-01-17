// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkGetNextConnectedPlayerProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get the next connected player properties. \nEach time this action is called it gets the next child of a GameObject.This lets you quickly loop through all the connected player to perform actions on them.")]
  [ActionCategory(ActionCategory.Network)]
  public class NetworkGetNextConnectedPlayerProperties : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Event to send for looping.")]
    [ActionSection("Set up")]
    public FsmEvent loopEvent;
    [HutongGames.PlayMaker.Tooltip("Event to send when there are no more children.")]
    public FsmEvent finishedEvent;
    [HutongGames.PlayMaker.Tooltip("The player connection index.")]
    [UIHint(UIHint.Variable)]
    [ActionSection("Result")]
    public FsmInt index;
    [HutongGames.PlayMaker.Tooltip("Get the IP address of this player.")]
    [UIHint(UIHint.Variable)]
    public FsmString IpAddress;
    [HutongGames.PlayMaker.Tooltip("Get the port of this player.")]
    [UIHint(UIHint.Variable)]
    public FsmInt port;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Get the GUID for this player, used when connecting with NAT punchthrough.")]
    public FsmString guid;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Get the external IP address of the network interface. This will only be populated after some external connection has been made.")]
    public FsmString externalIPAddress;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Get the external port of the network interface. This will only be populated after some external connection has been made.")]
    public FsmInt externalPort;
    private int nextItemIndex;

    public override void Reset()
    {
      this.finishedEvent = (FsmEvent) null;
      this.loopEvent = (FsmEvent) null;
      this.index = (FsmInt) null;
      this.IpAddress = (FsmString) null;
      this.port = (FsmInt) null;
      this.guid = (FsmString) null;
      this.externalIPAddress = (FsmString) null;
      this.externalPort = (FsmInt) null;
    }

    public override void OnEnter()
    {
      this.DoGetNextPlayerProperties();
      this.Finish();
    }

    private void DoGetNextPlayerProperties()
    {
      if (this.nextItemIndex >= Network.connections.Length)
      {
        this.Fsm.Event(this.finishedEvent);
        this.nextItemIndex = 0;
      }
      else
      {
        NetworkPlayer connection = Network.connections[this.nextItemIndex];
        this.index.Value = this.nextItemIndex;
        this.IpAddress.Value = connection.ipAddress;
        this.port.Value = connection.port;
        this.guid.Value = connection.guid;
        this.externalIPAddress.Value = connection.externalIP;
        this.externalPort.Value = connection.externalPort;
        if (this.nextItemIndex >= Network.connections.Length)
        {
          this.Fsm.Event(this.finishedEvent);
          this.nextItemIndex = 0;
        }
        else
        {
          ++this.nextItemIndex;
          if (this.loopEvent == null)
            return;
          this.Fsm.Event(this.loopEvent);
        }
      }
    }
  }
}
