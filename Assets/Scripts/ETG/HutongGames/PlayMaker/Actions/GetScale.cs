using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the Scale of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
  [ActionCategory(ActionCategory.Transform)]
  public class GetScale : FsmStateAction
  {
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    public FsmVector3 vector;
    [UIHint(UIHint.Variable)]
    public FsmFloat xScale;
    [UIHint(UIHint.Variable)]
    public FsmFloat yScale;
    [UIHint(UIHint.Variable)]
    public FsmFloat zScale;
    public Space space;
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.vector = (FsmVector3) null;
      this.xScale = (FsmFloat) null;
      this.yScale = (FsmFloat) null;
      this.zScale = (FsmFloat) null;
      this.space = Space.World;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoGetScale();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetScale();

    private void DoGetScale()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      Vector3 vector3 = this.space != Space.World ? ownerDefaultTarget.transform.localScale : ownerDefaultTarget.transform.lossyScale;
      this.vector.Value = vector3;
      this.xScale.Value = vector3.x;
      this.yScale.Value = vector3.y;
      this.zScale.Value = vector3.z;
    }
  }
}
