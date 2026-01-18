#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Reverses the direction of a Vector2 Variable. Same as multiplying by -1.")]
  [ActionCategory(ActionCategory.Vector2)]
  public class Vector2Invert : FsmStateAction
  {
    [RequiredField]
    [UIHint(UIHint.Variable)]
    [Tooltip("The vector to invert")]
    public FsmVector2 vector2Variable;
    [Tooltip("Repeat every frame")]
    public bool everyFrame;

    public override void Reset()
    {
      this.vector2Variable = (FsmVector2) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.vector2Variable.Value *= -1f;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.vector2Variable.Value *= -1f;
  }
}
