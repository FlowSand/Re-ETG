// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkGetConnectionsCount
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Get the number of connected players.\n\nOn a client this returns 1 (the server).")]
[ActionCategory(ActionCategory.Network)]
public class NetworkGetConnectionsCount : FsmStateAction
{
  [HutongGames.PlayMaker.Tooltip("Number of connected players.")]
  [UIHint(UIHint.Variable)]
  public FsmInt connectionsCount;
  [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
  public bool everyFrame;

  public override void Reset()
  {
    this.connectionsCount = (FsmInt) null;
    this.everyFrame = true;
  }

  public override void OnEnter()
  {
    this.connectionsCount.Value = Network.connections.Length;
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.connectionsCount.Value = Network.connections.Length;
}
