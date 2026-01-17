// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkGetConnectedPlayerProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get connected player properties.")]
  [ActionCategory(ActionCategory.Network)]
  public class NetworkGetConnectedPlayerProperties : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The player connection index.")]
    [RequiredField]
    public FsmInt index;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Get the IP address of this player.")]
    [ActionSection("Result")]
    public FsmString IpAddress;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Get the port of this player.")]
    public FsmInt port;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Get the GUID for this player, used when connecting with NAT punchthrough.")]
    public FsmString guid;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Get the external IP address of the network interface. This will only be populated after some external connection has been made.")]
    public FsmString externalIPAddress;
    [HutongGames.PlayMaker.Tooltip("Get the external port of the network interface. This will only be populated after some external connection has been made.")]
    [UIHint(UIHint.Variable)]
    public FsmInt externalPort;

    public override void Reset()
    {
      this.index = (FsmInt) null;
      this.IpAddress = (FsmString) null;
      this.port = (FsmInt) null;
      this.guid = (FsmString) null;
      this.externalIPAddress = (FsmString) null;
      this.externalPort = (FsmInt) null;
    }

    public override void OnEnter()
    {
      this.getPlayerProperties();
      this.Finish();
    }

    private void getPlayerProperties()
    {
      int index = this.index.Value;
      if (index < 0 || index >= Network.connections.Length)
      {
        this.LogError("Player index out of range");
      }
      else
      {
        NetworkPlayer connection = Network.connections[index];
        this.IpAddress.Value = connection.ipAddress;
        this.port.Value = connection.port;
        this.guid.Value = connection.guid;
        this.externalIPAddress.Value = connection.externalIP;
        this.externalPort.Value = connection.externalPort;
      }
    }
  }
}
