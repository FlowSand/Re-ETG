// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkGetLocalPlayerProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Network)]
[HutongGames.PlayMaker.Tooltip("Get the local network player properties")]
public class NetworkGetLocalPlayerProperties : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The IP address of this player.")]
  [UIHint(UIHint.Variable)]
  public FsmString IpAddress;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The port of this player.")]
  public FsmInt port;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The GUID for this player, used when connecting with NAT punchthrough.")]
  public FsmString guid;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The external IP address of the network interface. This will only be populated after some external connection has been made.")]
  public FsmString externalIPAddress;
  [HutongGames.PlayMaker.Tooltip("Returns the external port of the network interface. This will only be populated after some external connection has been made.")]
  [UIHint(UIHint.Variable)]
  public FsmInt externalPort;

  public override void Reset()
  {
    this.IpAddress = (FsmString) null;
    this.port = (FsmInt) null;
    this.guid = (FsmString) null;
    this.externalIPAddress = (FsmString) null;
    this.externalPort = (FsmInt) null;
  }

  public override void OnEnter()
  {
    this.IpAddress.Value = Network.player.ipAddress;
    this.port.Value = Network.player.port;
    this.guid.Value = Network.player.guid;
    this.externalIPAddress.Value = Network.player.externalIP;
    this.externalPort.Value = Network.player.externalPort;
    this.Finish();
  }
}
