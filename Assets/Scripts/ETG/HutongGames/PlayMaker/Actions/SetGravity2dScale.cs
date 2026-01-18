using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Physics2D)]
  [HutongGames.PlayMaker.Tooltip("Sets The degree to which this object is affected by gravity.  NOTE: Game object must have a rigidbody 2D.")]
  public class SetGravity2dScale : ComponentAction<Rigidbody2D>
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject with a Rigidbody 2d attached")]
    [CheckForComponent(typeof (Rigidbody2D))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The gravity scale effect")]
    [RequiredField]
    public FsmFloat gravityScale;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.gravityScale = (FsmFloat) 1f;
    }

    public override void OnEnter()
    {
      this.DoSetGravityScale();
      this.Finish();
    }

    private void DoSetGravityScale()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      this.rigidbody2d.gravityScale = this.gravityScale.Value;
    }
  }
}
