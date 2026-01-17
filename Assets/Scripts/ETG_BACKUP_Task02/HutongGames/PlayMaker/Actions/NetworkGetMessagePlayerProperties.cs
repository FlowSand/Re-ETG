// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkGetMessagePlayerProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Network)]
[HutongGames.PlayMaker.Tooltip("Get the network OnPlayerConnected or OnPlayerDisConnected message player info.")]
public class NetworkGetMessagePlayerProperties : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Get the IP address of this connected player.")]
  public FsmString IpAddress;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Get the port of this connected player.")]
  public FsmInt port;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Get the GUID for this connected player, used when connecting with NAT punchthrough.")]
  public FsmString guid;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Get the external IP address of the network interface. This will only be populated after some external connection has been made.")]
  public FsmString externalIPAddress;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Get the external port of the network interface. This will only be populated after some external connection has been made.")]
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
    this.doGetOnPLayerConnectedProperties();
    this.Finish();
  }

  private void doGetOnPLayerConnectedProperties()
  {
    NetworkPlayer player = Fsm.EventData.Player;
    Debug.Log((object) ("hello " + player.ipAddress));
    this.IpAddress.Value = player.ipAddress;
    this.port.Value = player.port;
    this.guid.Value = player.guid;
    this.externalIPAddress.Value = player.externalIP;
    this.externalPort.Value = player.externalPort;
    this.Finish();
  }
}
