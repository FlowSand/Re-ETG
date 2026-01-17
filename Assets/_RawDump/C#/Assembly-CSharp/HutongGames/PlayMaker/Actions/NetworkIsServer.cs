// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkIsServer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Network)]
[HutongGames.PlayMaker.Tooltip("Test if your peer type is server.")]
public class NetworkIsServer : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("True if running as server.")]
  public FsmBool isServer;
  [HutongGames.PlayMaker.Tooltip("Event to send if running as server.")]
  public FsmEvent isServerEvent;
  [HutongGames.PlayMaker.Tooltip("Event to send if not running as server.")]
  public FsmEvent isNotServerEvent;

  public override void Reset() => this.isServer = (FsmBool) null;

  public override void OnEnter()
  {
    this.DoCheckIsServer();
    this.Finish();
  }

  private void DoCheckIsServer()
  {
    this.isServer.Value = Network.isServer;
    if (Network.isServer && this.isServerEvent != null)
    {
      this.Fsm.Event(this.isServerEvent);
    }
    else
    {
      if (Network.isServer || this.isNotServerEvent == null)
        return;
      this.Fsm.Event(this.isNotServerEvent);
    }
  }
}
