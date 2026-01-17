// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkGetTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Get the current network time (seconds).")]
  [ActionCategory(ActionCategory.Network)]
  public class NetworkGetTime : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The network time.")]
    [UIHint(UIHint.Variable)]
    public FsmFloat time;

    public override void Reset() => this.time = (FsmFloat) null;

    public override void OnEnter()
    {
      this.time.Value = (float) Network.time;
      this.Finish();
    }
  }
}
