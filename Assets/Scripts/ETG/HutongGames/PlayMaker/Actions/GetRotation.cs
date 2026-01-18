using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the Rotation of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
  [ActionCategory(ActionCategory.Transform)]
  public class GetRotation : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    public FsmQuaternion quaternion;
    [Title("Euler Angles")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 vector;
    [UIHint(UIHint.Variable)]
    public FsmFloat xAngle;
    [UIHint(UIHint.Variable)]
    public FsmFloat yAngle;
    [UIHint(UIHint.Variable)]
    public FsmFloat zAngle;
    public Space space;
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.quaternion = (FsmQuaternion) null;
      this.vector = (FsmVector3) null;
      this.xAngle = (FsmFloat) null;
      this.yAngle = (FsmFloat) null;
      this.zAngle = (FsmFloat) null;
      this.space = Space.World;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoGetRotation();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetRotation();

    private void DoGetRotation()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      if (this.space == Space.World)
      {
        this.quaternion.Value = ownerDefaultTarget.transform.rotation;
        Vector3 eulerAngles = ownerDefaultTarget.transform.eulerAngles;
        this.vector.Value = eulerAngles;
        this.xAngle.Value = eulerAngles.x;
        this.yAngle.Value = eulerAngles.y;
        this.zAngle.Value = eulerAngles.z;
      }
      else
      {
        Vector3 localEulerAngles = ownerDefaultTarget.transform.localEulerAngles;
        this.quaternion.Value = Quaternion.Euler(localEulerAngles);
        this.vector.Value = localEulerAngles;
        this.xAngle.Value = localEulerAngles.x;
        this.yAngle.Value = localEulerAngles.y;
        this.zAngle.Value = localEulerAngles.z;
      }
    }
  }
}
