// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.MasterServerSetProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Network)]
  [HutongGames.PlayMaker.Tooltip("Set the IP address, port, update rate and dedicated server flag of the master server.")]
  public class MasterServerSetProperties : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Set the IP address of the master server.")]
    public FsmString ipAddress;
    [HutongGames.PlayMaker.Tooltip("Set the connection port of the master server.")]
    public FsmInt port;
    [HutongGames.PlayMaker.Tooltip("Set the minimum update rate for master server host information update. Default is 60 seconds.")]
    public FsmInt updateRate;
    [HutongGames.PlayMaker.Tooltip("Set if this machine is a dedicated server.")]
    public FsmBool dedicatedServer;

    public override void Reset()
    {
      this.ipAddress = (FsmString) "127.0.0.1";
      this.port = (FsmInt) 10002;
      this.updateRate = (FsmInt) 60;
      this.dedicatedServer = (FsmBool) false;
    }

    public override void OnEnter()
    {
      this.SetMasterServerProperties();
      this.Finish();
    }

    private void SetMasterServerProperties()
    {
      MasterServer.ipAddress = this.ipAddress.Value;
      MasterServer.port = this.port.Value;
      MasterServer.updateRate = this.updateRate.Value;
      MasterServer.dedicatedServer = this.dedicatedServer.Value;
    }
  }
}
