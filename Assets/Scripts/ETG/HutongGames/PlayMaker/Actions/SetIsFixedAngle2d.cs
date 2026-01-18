using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Physics2D)]
  [HutongGames.PlayMaker.Tooltip("Controls whether the rigidbody 2D should be prevented from rotating")]
  [Obsolete("This action is obsolete; use Constraints instead.")]
  public class SetIsFixedAngle2d : ComponentAction<Rigidbody2D>
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject with the Rigidbody2D attached")]
    [CheckForComponent(typeof (Rigidbody2D))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The flag value")]
    [RequiredField]
    public FsmBool isFixedAngle;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.isFixedAngle = (FsmBool) false;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetIsFixedAngle();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetIsFixedAngle();

    private void DoSetIsFixedAngle()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      if (this.isFixedAngle.Value)
        this.rigidbody2d.constraints |= RigidbodyConstraints2D.FreezeRotation;
      else
        this.rigidbody2d.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
    }
  }
}
