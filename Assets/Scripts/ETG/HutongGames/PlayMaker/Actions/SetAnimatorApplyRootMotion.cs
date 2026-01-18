using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Set Apply Root Motion: If true, Root is controlled by animations")]
  [ActionCategory(ActionCategory.Animator)]
  public class SetAnimatorApplyRootMotion : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
    [CheckForComponent(typeof (Animator))]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("If true, Root is controlled by animations")]
    public FsmBool applyRootMotion;
    private Animator _animator;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.applyRootMotion = (FsmBool) null;
    }

    public override void OnEnter()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
      {
        this.Finish();
      }
      else
      {
        this._animator = ownerDefaultTarget.GetComponent<Animator>();
        if ((Object) this._animator == (Object) null)
        {
          this.Finish();
        }
        else
        {
          this.DoApplyRootMotion();
          this.Finish();
        }
      }
    }

    private void DoApplyRootMotion()
    {
      if ((Object) this._animator == (Object) null)
        return;
      this._animator.applyRootMotion = this.applyRootMotion.Value;
    }
  }
}
