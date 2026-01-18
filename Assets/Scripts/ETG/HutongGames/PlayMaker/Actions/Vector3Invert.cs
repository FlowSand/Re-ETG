#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Vector3)]
  [Tooltip("Reverses the direction of a Vector3 Variable. Same as multiplying by -1.")]
  public class Vector3Invert : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmVector3 vector3Variable;
    public bool everyFrame;

    public override void Reset()
    {
      this.vector3Variable = (FsmVector3) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.vector3Variable.Value *= -1f;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.vector3Variable.Value *= -1f;
  }
}
