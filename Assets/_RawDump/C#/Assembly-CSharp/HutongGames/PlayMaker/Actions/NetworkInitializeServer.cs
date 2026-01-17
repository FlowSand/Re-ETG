// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkInitializeServer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Launch a server.")]
[ActionCategory(ActionCategory.Network)]
public class NetworkInitializeServer : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The number of allowed incoming connections/number of players allowed in the game.")]
  [RequiredField]
  public FsmInt connections;
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("The port number we want to listen to.")]
  public FsmInt listenPort;
  [HutongGames.PlayMaker.Tooltip("Sets the password for the server. This must be matched in the NetworkConnect action.")]
  public FsmString incomingPassword;
  [HutongGames.PlayMaker.Tooltip("Sets the NAT punchthrough functionality.")]
  public FsmBool useNAT;
  [HutongGames.PlayMaker.Tooltip("Unity handles the network layer by providing secure connections if you wish to use them. \nMost games will want to use secure connections. However, they add up to 15 bytes per packet and take time to compute so you may wish to limit usage to deployed games only.")]
  public FsmBool useSecurityLayer;
  [HutongGames.PlayMaker.Tooltip("Run the server in the background, even if it doesn't have focus.")]
  public FsmBool runInBackground;
  [HutongGames.PlayMaker.Tooltip("Event to send in case of an error creating the server.")]
  [ActionSection("Errors")]
  public FsmEvent errorEvent;
  [HutongGames.PlayMaker.Tooltip("Store the error string in a variable.")]
  [UIHint(UIHint.Variable)]
  public FsmString errorString;

  public override void Reset()
  {
    this.connections = (FsmInt) 32 /*0x20*/;
    this.listenPort = (FsmInt) 25001;
    this.incomingPassword = (FsmString) string.Empty;
    this.errorEvent = (FsmEvent) null;
    this.errorString = (FsmString) null;
    this.useNAT = (FsmBool) false;
    this.useSecurityLayer = (FsmBool) false;
    this.runInBackground = (FsmBool) true;
  }

  public override void OnEnter()
  {
    Network.incomingPassword = this.incomingPassword.Value;
    if (this.useSecurityLayer.Value)
      Network.InitializeSecurity();
    if (this.runInBackground.Value)
      Application.runInBackground = true;
    NetworkConnectionError networkConnectionError = Network.InitializeServer(this.connections.Value, this.listenPort.Value, this.useNAT.Value);
    if (networkConnectionError != NetworkConnectionError.NoError)
    {
      this.errorString.Value = networkConnectionError.ToString();
      this.LogError(this.errorString.Value);
      this.Fsm.Event(this.errorEvent);
    }
    this.Finish();
  }
}
