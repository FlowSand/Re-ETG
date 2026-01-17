// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkSetMaximumConnections
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Set the maximum amount of connections/players allowed.\n\nThis cannot be set higher than the connection count given in Launch Server.\n\nSetting it to 0 means no new connections can be made but the existing ones stay connected.\n\nSetting it to -1 means the maximum connections count is set to the same number of current open connections. In that case, if a players drops then the slot is still open for him.")]
[ActionCategory(ActionCategory.Network)]
public class NetworkSetMaximumConnections : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("The maximum amount of connections/players allowed.")]
  public FsmInt maximumConnections;

  public override void Reset() => this.maximumConnections = (FsmInt) 32 /*0x20*/;

  public override void OnEnter()
  {
    if (this.maximumConnections.Value < -1)
    {
      this.LogWarning("Network Maximum connections can not be less than -1");
      this.maximumConnections.Value = -1;
    }
    Network.maxConnections = this.maximumConnections.Value;
    this.Finish();
  }

  public override string ErrorCheck()
  {
    return this.maximumConnections.Value < -1 ? "Network Maximum connections can not be less than -1" : string.Empty;
  }
}
