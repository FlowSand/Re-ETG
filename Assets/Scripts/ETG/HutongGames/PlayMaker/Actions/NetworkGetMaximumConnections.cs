using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
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
}
