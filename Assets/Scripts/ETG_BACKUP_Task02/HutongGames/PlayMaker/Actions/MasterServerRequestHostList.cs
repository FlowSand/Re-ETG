// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.MasterServerRequestHostList
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Network)]
[HutongGames.PlayMaker.Tooltip("Request a host list from the master server.\n\nUse MasterServer Get Host Data to get info on each host in the host list.")]
public class MasterServerRequestHostList : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The unique game type name.")]
  [RequiredField]
  public FsmString gameTypeName;
  [HutongGames.PlayMaker.Tooltip("Event sent when the host list has arrived. NOTE: The action will not Finish until the host list arrives.")]
  public FsmEvent HostListArrivedEvent;

  public override void Reset()
  {
    this.gameTypeName = (FsmString) null;
    this.HostListArrivedEvent = (FsmEvent) null;
  }

  public override void OnEnter() => this.DoMasterServerRequestHost();

  public override void OnUpdate() => this.WatchServerRequestHost();

  private void DoMasterServerRequestHost()
  {
    MasterServer.ClearHostList();
    MasterServer.RequestHostList(this.gameTypeName.Value);
  }

  private void WatchServerRequestHost()
  {
    if (MasterServer.PollHostList().Length == 0)
      return;
    this.Fsm.Event(this.HostListArrivedEvent);
    this.Finish();
  }
}
