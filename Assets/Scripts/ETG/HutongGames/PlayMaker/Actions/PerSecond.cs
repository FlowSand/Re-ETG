using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Multiplies a Float by Time.deltaTime to use in frame-rate independent operations. E.g., 10 becomes 10 units per second.")]
  [ActionCategory(ActionCategory.Time)]
  public class PerSecond : FsmStateAction
  {
    [RequiredField]
    public FsmFloat floatValue;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmFloat storeResult;
    public bool everyFrame;

    public override void Reset()
    {
      this.floatValue = (FsmFloat) null;
      this.storeResult = (FsmFloat) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoPerSecond();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoPerSecond();

    private void DoPerSecond()
    {
      if (this.storeResult == null)
        return;
      this.storeResult.Value = this.floatValue.Value * Time.deltaTime;
    }
  }
}
