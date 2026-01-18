using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Transform)]
  [HutongGames.PlayMaker.Tooltip("Gets the Angle between a GameObject's forward axis and a Target. The Target can be defined as a GameObject or a world Position. If you specify both, then the Position will be used as a local offset from the Target Object's position.")]
  public class GetAngleToTarget : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The game object whose forward axis we measure from. If the target is dead ahead the angle will be 0.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The target object to measure the angle to. Or use target position.")]
    public FsmGameObject targetObject;
    [HutongGames.PlayMaker.Tooltip("The world position to measure an angle to. If Target Object is also specified, this vector is used as an offset from that object's position.")]
    public FsmVector3 targetPosition;
    [HutongGames.PlayMaker.Tooltip("Ignore height differences when calculating the angle.")]
    public FsmBool ignoreHeight;
    [RequiredField]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Store the angle in a float variable.")]
    public FsmFloat storeAngle;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.targetObject = (FsmGameObject) null;
      FsmVector3 fsmVector3 = new FsmVector3();
      fsmVector3.UseVariable = true;
      this.targetPosition = fsmVector3;
      this.ignoreHeight = (FsmBool) true;
      this.storeAngle = (FsmFloat) null;
      this.everyFrame = false;
    }

    public override void OnLateUpdate()
    {
      this.DoGetAngleToTarget();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    private void DoGetAngleToTarget()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      GameObject gameObject = this.targetObject.Value;
      if ((Object) gameObject == (Object) null && this.targetPosition.IsNone)
        return;
      Vector3 vector3 = !((Object) gameObject != (Object) null) ? this.targetPosition.Value : (this.targetPosition.IsNone ? gameObject.transform.position : gameObject.transform.TransformPoint(this.targetPosition.Value));
      if (this.ignoreHeight.Value)
        vector3.y = ownerDefaultTarget.transform.position.y;
      this.storeAngle.Value = Vector3.Angle(vector3 - ownerDefaultTarget.transform.position, ownerDefaultTarget.transform.forward);
    }
  }
}
