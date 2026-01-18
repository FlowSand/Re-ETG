using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Clamp the value of an Integer Variable to a Min/Max range.")]
  [ActionCategory(ActionCategory.Math)]
  public class IntClamp : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmInt intVariable;
    [RequiredField]
    public FsmInt minValue;
    [RequiredField]
    public FsmInt maxValue;
    public bool everyFrame;

    public override void Reset()
    {
      this.intVariable = (FsmInt) null;
      this.minValue = (FsmInt) null;
      this.maxValue = (FsmInt) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoClamp();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoClamp();

    private void DoClamp()
    {
      this.intVariable.Value = Mathf.Clamp(this.intVariable.Value, this.minValue.Value, this.maxValue.Value);
    }
  }
}
