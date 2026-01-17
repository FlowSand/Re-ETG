// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkIsClient
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Network)]
[HutongGames.PlayMaker.Tooltip("Test if your peer type is client.")]
public class NetworkIsClient : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("True if running as client.")]
  public FsmBool isClient;
  [HutongGames.PlayMaker.Tooltip("Event to send if running as client.")]
  public FsmEvent isClientEvent;
  [HutongGames.PlayMaker.Tooltip("Event to send if not running as client.")]
  public FsmEvent isNotClientEvent;

  public override void Reset() => this.isClient = (FsmBool) null;

  public override void OnEnter()
  {
    this.DoCheckIsClient();
    this.Finish();
  }

  private void DoCheckIsClient()
  {
    this.isClient.Value = Network.isClient;
    if (Network.isClient && this.isClientEvent != null)
    {
      this.Fsm.Event(this.isClientEvent);
    }
    else
    {
      if (Network.isClient || this.isNotClientEvent == null)
        return;
      this.Fsm.Event(this.isNotClientEvent);
    }
  }
}
