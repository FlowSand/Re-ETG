using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Clamps the Magnitude of Vector3 Variable.")]
  [ActionCategory(ActionCategory.Vector3)]
  public class Vector3ClampMagnitude : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmVector3 vector3Variable;
    [RequiredField]
    public FsmFloat maxLength;
    public bool everyFrame;

    public override void Reset()
    {
      this.vector3Variable = (FsmVector3) null;
      this.maxLength = (FsmFloat) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoVector3ClampMagnitude();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoVector3ClampMagnitude();

    private void DoVector3ClampMagnitude()
    {
      this.vector3Variable.Value = Vector3.ClampMagnitude(this.vector3Variable.Value, this.maxLength.Value);
    }
  }
}
