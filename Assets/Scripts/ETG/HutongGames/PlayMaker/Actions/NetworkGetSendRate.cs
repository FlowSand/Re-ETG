// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkGetSendRate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Store the current send rate for all NetworkViews")]
  [ActionCategory(ActionCategory.Network)]
  public class NetworkGetSendRate : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the current send rate for NetworkViews")]
    [RequiredField]
    public FsmFloat sendRate;

    public override void Reset() => this.sendRate = (FsmFloat) null;

    public override void OnEnter()
    {
      this.DoGetSendRate();
      this.Finish();
    }

    private void DoGetSendRate() => this.sendRate.Value = Network.sendRate;
  }
}
