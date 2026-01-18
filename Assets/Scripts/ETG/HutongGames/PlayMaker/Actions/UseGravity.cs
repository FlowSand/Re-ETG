using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets whether a Game Object's Rigidy Body is affected by Gravity.")]
  [ActionCategory(ActionCategory.Physics)]
  public class UseGravity : ComponentAction<Rigidbody>
  {
    [CheckForComponent(typeof (Rigidbody))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    public FsmBool useGravity;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.useGravity = (FsmBool) true;
    }

    public override void OnEnter()
    {
      this.DoUseGravity();
      this.Finish();
    }

    private void DoUseGravity()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      this.rigidbody.useGravity = this.useGravity.Value;
    }
  }
}
