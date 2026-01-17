// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkSetLogLevel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Network)]
  [HutongGames.PlayMaker.Tooltip("Set the log level for network messages. Default is Off.\n\nOff: Only report errors, otherwise silent.\n\nInformational: Report informational messages like connectivity events.\n\nFull: Full debug level logging down to each individual message being reported.")]
  public class NetworkSetLogLevel : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The log level")]
    public NetworkLogLevel logLevel;

    public override void Reset() => this.logLevel = NetworkLogLevel.Off;

    public override void OnEnter()
    {
      Network.logLevel = this.logLevel;
      this.Finish();
    }
  }
}
