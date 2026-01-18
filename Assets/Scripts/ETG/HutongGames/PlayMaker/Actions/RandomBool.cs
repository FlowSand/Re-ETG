using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Math)]
  [HutongGames.PlayMaker.Tooltip("Sets a Bool Variable to True or False randomly.")]
  public class RandomBool : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    public FsmBool storeResult;

    public override void Reset() => this.storeResult = (FsmBool) null;

    public override void OnEnter()
    {
      this.storeResult.Value = Random.Range(0, 100) < 50;
      this.Finish();
    }
  }
}
