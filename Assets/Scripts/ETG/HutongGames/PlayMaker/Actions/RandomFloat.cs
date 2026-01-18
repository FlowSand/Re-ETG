using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Math)]
  [HutongGames.PlayMaker.Tooltip("Sets a Float Variable to a random value between Min/Max.")]
  public class RandomFloat : FsmStateAction
  {
    [RequiredField]
    public FsmFloat min;
    [RequiredField]
    public FsmFloat max;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmFloat storeResult;

    public override void Reset()
    {
      this.min = (FsmFloat) 0.0f;
      this.max = (FsmFloat) 1f;
      this.storeResult = (FsmFloat) null;
    }

    public override void OnEnter()
    {
      this.storeResult.Value = Random.Range(this.min.Value, this.max.Value);
      this.Finish();
    }
  }
}
