#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Normalizes a Vector2 Variable.")]
  [ActionCategory(ActionCategory.Vector2)]
  public class Vector2Normalize : FsmStateAction
  {
    [Tooltip("The vector to normalize")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
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
      this.vector2Variable.Value = this.vector2Variable.Value.normalized;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      this.vector2Variable.Value = this.vector2Variable.Value.normalized;
    }
  }
}
