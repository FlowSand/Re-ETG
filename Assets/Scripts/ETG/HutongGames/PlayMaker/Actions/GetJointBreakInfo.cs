#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Gets info on the last joint break event.")]
  [ActionCategory(ActionCategory.Physics)]
  public class GetJointBreakInfo : FsmStateAction
  {
    [Tooltip("Get the force that broke the joint.")]
    [UIHint(UIHint.Variable)]
    public FsmFloat breakForce;

    public override void Reset() => this.breakForce = (FsmFloat) null;

    public override void OnEnter()
    {
      this.breakForce.Value = this.Fsm.JointBreakForce;
      this.Finish();
    }
  }
}
