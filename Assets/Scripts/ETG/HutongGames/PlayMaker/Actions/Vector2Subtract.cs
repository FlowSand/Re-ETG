#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Subtracts a Vector2 value from a Vector2 variable.")]
  [ActionCategory(ActionCategory.Vector2)]
  public class Vector2Subtract : FsmStateAction
  {
    [Tooltip("The Vector2 operand")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmVector2 vector2Variable;
    [Tooltip("The vector2 to substract with")]
    [RequiredField]
    public FsmVector2 subtractVector;
    [Tooltip("Repeat every frame")]
    public bool everyFrame;

    public override void Reset()
    {
      this.vector2Variable = (FsmVector2) null;
      FsmVector2 fsmVector2 = new FsmVector2();
      fsmVector2.UseVariable = true;
      this.subtractVector = fsmVector2;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.vector2Variable.Value -= this.subtractVector.Value;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.vector2Variable.Value -= this.subtractVector.Value;
  }
}
