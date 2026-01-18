using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the individual fields of a Rect Variable. To leave any field unchanged, set variable to 'None'.")]
  [ActionCategory(ActionCategory.Rect)]
  public class SetRectFields : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmRect rectVariable;
    public FsmFloat x;
    public FsmFloat y;
    public FsmFloat width;
    public FsmFloat height;
    public bool everyFrame;

    public override void Reset()
    {
      this.rectVariable = (FsmRect) null;
      FsmFloat fsmFloat1 = new FsmFloat();
      fsmFloat1.UseVariable = true;
      this.x = fsmFloat1;
      FsmFloat fsmFloat2 = new FsmFloat();
      fsmFloat2.UseVariable = true;
      this.y = fsmFloat2;
      FsmFloat fsmFloat3 = new FsmFloat();
      fsmFloat3.UseVariable = true;
      this.width = fsmFloat3;
      FsmFloat fsmFloat4 = new FsmFloat();
      fsmFloat4.UseVariable = true;
      this.height = fsmFloat4;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetRectFields();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetRectFields();

    private void DoSetRectFields()
    {
      if (this.rectVariable.IsNone)
        return;
      Rect rect = this.rectVariable.Value;
      if (!this.x.IsNone)
        rect.x = this.x.Value;
      if (!this.y.IsNone)
        rect.y = this.y.Value;
      if (!this.width.IsNone)
        rect.width = this.width.Value;
      if (!this.height.IsNone)
        rect.height = this.height.Value;
      this.rectVariable.Value = rect;
    }
  }
}
