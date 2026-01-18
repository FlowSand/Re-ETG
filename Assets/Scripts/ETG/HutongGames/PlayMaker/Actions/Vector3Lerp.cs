using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Linearly interpolates between 2 vectors.")]
  [ActionCategory(ActionCategory.Vector3)]
  public class Vector3Lerp : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("First Vector.")]
    [RequiredField]
    public FsmVector3 fromVector;
    [HutongGames.PlayMaker.Tooltip("Second Vector.")]
    [RequiredField]
    public FsmVector3 toVector;
    [HutongGames.PlayMaker.Tooltip("Interpolate between From Vector and ToVector by this amount. Value is clamped to 0-1 range. 0 = From Vector; 1 = To Vector; 0.5 = half way between.")]
    [RequiredField]
    public FsmFloat amount;
    [HutongGames.PlayMaker.Tooltip("Store the result in this vector variable.")]
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmVector3 storeResult;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if any of the values are changing.")]
    public bool everyFrame;

    public override void Reset()
    {
      FsmVector3 fsmVector3_1 = new FsmVector3();
      fsmVector3_1.UseVariable = true;
      this.fromVector = fsmVector3_1;
      FsmVector3 fsmVector3_2 = new FsmVector3();
      fsmVector3_2.UseVariable = true;
      this.toVector = fsmVector3_2;
      this.storeResult = (FsmVector3) null;
      this.everyFrame = true;
    }

    public override void OnEnter()
    {
      this.DoVector3Lerp();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoVector3Lerp();

    private void DoVector3Lerp()
    {
      this.storeResult.Value = Vector3.Lerp(this.fromVector.Value, this.toVector.Value, this.amount.Value);
    }
  }
}
