// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkConnect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Connect to a server.")]
  [ActionCategory(ActionCategory.Network)]
  public class NetworkConnect : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("IP address of the host. Either a dotted IP address or a domain name.")]
    [RequiredField]
    public FsmString remoteIP;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The port on the remote machine to connect to.")]
    public FsmInt remotePort;
    [HutongGames.PlayMaker.Tooltip("Optional password for the server.")]
    public FsmString password;
    [HutongGames.PlayMaker.Tooltip("Event to send in case of an error connecting to the server.")]
    [ActionSection("Errors")]
    public FsmEvent errorEvent;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the error string in a variable.")]
    public FsmString errorString;

    public override void Reset()
    {
      this.remoteIP = (FsmString) "127.0.0.1";
      this.remotePort = (FsmInt) 25001;
      this.password = (FsmString) string.Empty;
      this.errorEvent = (FsmEvent) null;
      this.errorString = (FsmString) null;
    }

    public override void OnEnter()
    {
      NetworkConnectionError networkConnectionError = Network.Connect(this.remoteIP.Value, this.remotePort.Value, this.password.Value);
      if (networkConnectionError != NetworkConnectionError.NoError)
      {
        this.errorString.Value = networkConnectionError.ToString();
        this.LogError(this.errorString.Value);
        this.Fsm.Event(this.errorEvent);
      }
      this.Finish();
    }
  }
}
