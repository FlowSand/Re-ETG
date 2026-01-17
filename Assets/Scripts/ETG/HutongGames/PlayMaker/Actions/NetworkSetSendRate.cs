// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NetworkSetSendRate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Set the send rate for all networkViews. Default is 15")]
  [ActionCategory(ActionCategory.Network)]
  public class NetworkSetSendRate : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The send rate for all networkViews")]
    [RequiredField]
    public FsmFloat sendRate;

    public override void Reset() => this.sendRate = (FsmFloat) 15f;

    public override void OnEnter()
    {
      this.DoSetSendRate();
      this.Finish();
    }

    private void DoSetSendRate() => Network.sendRate = this.sendRate.Value;
  }
}
