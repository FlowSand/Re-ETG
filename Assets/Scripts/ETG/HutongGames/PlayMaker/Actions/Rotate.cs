using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Transform)]
  [HutongGames.PlayMaker.Tooltip("Rotates a Game Object around each Axis. Use a Vector3 Variable and/or XYZ components. To leave any axis unchanged, set variable to 'None'.")]
  public class Rotate : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The game object to rotate.")]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("A rotation vector. NOTE: You can override individual axis below.")]
    public FsmVector3 vector;
    [HutongGames.PlayMaker.Tooltip("Rotation around x axis.")]
    public FsmFloat xAngle;
    [HutongGames.PlayMaker.Tooltip("Rotation around y axis.")]
    public FsmFloat yAngle;
    [HutongGames.PlayMaker.Tooltip("Rotation around z axis.")]
    public FsmFloat zAngle;
    [HutongGames.PlayMaker.Tooltip("Rotate in local or world space.")]
    public Space space;
    [HutongGames.PlayMaker.Tooltip("Rotate over one second")]
    public bool perSecond;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;
    [HutongGames.PlayMaker.Tooltip("Perform the rotation in LateUpdate. This is useful if you want to override the rotation of objects that are animated or otherwise rotated in Update.")]
    public bool lateUpdate;
    [HutongGames.PlayMaker.Tooltip("Perform the rotation in FixedUpdate. This is useful when working with rigid bodies and physics.")]
    public bool fixedUpdate;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.vector = (FsmVector3) null;
      FsmFloat fsmFloat1 = new FsmFloat();
      fsmFloat1.UseVariable = true;
      this.xAngle = fsmFloat1;
      FsmFloat fsmFloat2 = new FsmFloat();
      fsmFloat2.UseVariable = true;
      this.yAngle = fsmFloat2;
      FsmFloat fsmFloat3 = new FsmFloat();
      fsmFloat3.UseVariable = true;
      this.zAngle = fsmFloat3;
      this.space = Space.Self;
      this.perSecond = false;
      this.everyFrame = true;
      this.lateUpdate = false;
      this.fixedUpdate = false;
    }

    public override void OnPreprocess() => this.Fsm.HandleFixedUpdate = true;

    public override void OnEnter()
    {
      if (this.everyFrame || this.lateUpdate || this.fixedUpdate)
        return;
      this.DoRotate();
      this.Finish();
    }

    public override void OnUpdate()
    {
      if (this.lateUpdate || this.fixedUpdate)
        return;
      this.DoRotate();
    }

    public override void OnLateUpdate()
    {
      if (this.lateUpdate)
        this.DoRotate();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnFixedUpdate()
    {
      if (this.fixedUpdate)
        this.DoRotate();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    private void DoRotate()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      Vector3 eulerAngles = !this.vector.IsNone ? this.vector.Value : new Vector3(this.xAngle.Value, this.yAngle.Value, this.zAngle.Value);
      if (!this.xAngle.IsNone)
        eulerAngles.x = this.xAngle.Value;
      if (!this.yAngle.IsNone)
        eulerAngles.y = this.yAngle.Value;
      if (!this.zAngle.IsNone)
        eulerAngles.z = this.zAngle.Value;
      if (!this.perSecond)
        ownerDefaultTarget.transform.Rotate(eulerAngles, this.space);
      else
        ownerDefaultTarget.transform.Rotate(eulerAngles * Time.deltaTime, this.space);
    }
  }
}
