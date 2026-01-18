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
