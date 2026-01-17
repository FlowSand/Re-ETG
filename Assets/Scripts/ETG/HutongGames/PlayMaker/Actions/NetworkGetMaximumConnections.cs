// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkGetMaximumConnections
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Get the maximum amount of connections/players allowed.")]
[ActionCategory(ActionCategory.Network)]
public class NetworkGetMaximumConnections : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  [HutongGames.PlayMaker.Tooltip("Get the maximum amount of connections/players allowed.")]
  public FsmInt result;

  public override void Reset() => this.result = (FsmInt) null;

  public override void OnEnter()
  {
    this.result.Value = Network.maxConnections;
    this.Finish();
  }
}
