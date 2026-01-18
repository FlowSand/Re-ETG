#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Vector3)]
  [Tooltip("Get Vector3 Length.")]
  public class GetVectorLength : FsmStateAction
  {
    public FsmVector3 vector3;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmFloat storeLength;

    public override void Reset()
    {
      this.vector3 = (FsmVector3) null;
      this.storeLength = (FsmFloat) null;
    }

    public override void OnEnter()
    {
      this.DoVectorLength();
      this.Finish();
    }

    private void DoVectorLength()
    {
      if (this.vector3 == null || this.storeLength == null)
        return;
      this.storeLength.Value = this.vector3.Value.magnitude;
    }
  }
}
