// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.MasterServerGetProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Get the IP address, port, update rate and dedicated server flag of the master server and store in variables.")]
[ActionCategory(ActionCategory.Network)]
public class MasterServerGetProperties : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The IP address of the master server.")]
  public FsmString ipAddress;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("The connection port of the master server.")]
  public FsmInt port;
  [HutongGames.PlayMaker.Tooltip("The minimum update rate for master server host information update. Default is 60 seconds")]
  [UIHint(UIHint.Variable)]
  public FsmInt updateRate;
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Flag to report if this machine is a dedicated server.")]
  public FsmBool dedicatedServer;
  [HutongGames.PlayMaker.Tooltip("Event sent if this machine is a dedicated server")]
  public FsmEvent isDedicatedServerEvent;
  [HutongGames.PlayMaker.Tooltip("Event sent if this machine is not a dedicated server")]
  public FsmEvent isNotDedicatedServerEvent;

  public override void Reset()
  {
    this.ipAddress = (FsmString) null;
    this.port = (FsmInt) null;
    this.updateRate = (FsmInt) null;
    this.dedicatedServer = (FsmBool) null;
    this.isDedicatedServerEvent = (FsmEvent) null;
    this.isNotDedicatedServerEvent = (FsmEvent) null;
  }

  public override void OnEnter()
  {
    this.GetMasterServerProperties();
    this.Finish();
  }

  private void GetMasterServerProperties()
  {
    this.ipAddress.Value = MasterServer.ipAddress;
    this.port.Value = MasterServer.port;
    this.updateRate.Value = MasterServer.updateRate;
    bool dedicatedServer = MasterServer.dedicatedServer;
    this.dedicatedServer.Value = dedicatedServer;
    if (dedicatedServer && this.isDedicatedServerEvent != null)
      this.Fsm.Event(this.isDedicatedServerEvent);
    if (dedicatedServer || this.isNotDedicatedServerEvent == null)
      return;
    this.Fsm.Event(this.isNotDedicatedServerEvent);
  }
}
