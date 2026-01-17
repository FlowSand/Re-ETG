// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.MasterServerUnregisterHost
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Network)]
  [HutongGames.PlayMaker.Tooltip("Unregister this server from the master server.\n\nDoes nothing if the server is not registered or has already unregistered.")]
  public class MasterServerUnregisterHost : FsmStateAction
  {
    public override void OnEnter()
    {
      MasterServer.UnregisterHost();
      this.Finish();
    }
  }
}
