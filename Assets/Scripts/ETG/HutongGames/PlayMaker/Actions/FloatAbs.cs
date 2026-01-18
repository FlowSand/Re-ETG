using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets a Float variable to its absolute value.")]
  [ActionCategory(ActionCategory.Math)]
  public class FloatAbs : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The Float variable.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmFloat floatVariable;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the Float variable is changing.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.floatVariable = (FsmFloat) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoFloatAbs();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoFloatAbs();

    private void DoFloatAbs() => this.floatVariable.Value = Mathf.Abs(this.floatVariable.Value);
  }
}
