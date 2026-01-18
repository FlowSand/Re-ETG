using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Animator)]
  [HutongGames.PlayMaker.Tooltip("Gets the value of ApplyRootMotion of an avatar. If true, root is controlled by animations")]
  public class GetAnimatorApplyRootMotion : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
    [CheckForComponent(typeof (Animator))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Is the rootMotionapplied. If true, root is controlled by animations")]
    [ActionSection("Results")]
    [RequiredField]
    public FsmBool rootMotionApplied;
    [HutongGames.PlayMaker.Tooltip("Event send if the root motion is applied")]
    public FsmEvent rootMotionIsAppliedEvent;
    [HutongGames.PlayMaker.Tooltip("Event send if the root motion is not applied")]
    public FsmEvent rootMotionIsNotAppliedEvent;
    private Animator _animator;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.rootMotionApplied = (FsmBool) null;
      this.rootMotionIsAppliedEvent = (FsmEvent) null;
      this.rootMotionIsNotAppliedEvent = (FsmEvent) null;
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
          this.GetApplyMotionRoot();
          this.Finish();
        }
      }
    }

    private void GetApplyMotionRoot()
    {
      if (!((Object) this._animator != (Object) null))
        return;
      bool applyRootMotion = this._animator.applyRootMotion;
      this.rootMotionApplied.Value = applyRootMotion;
      if (applyRootMotion)
        this.Fsm.Event(this.rootMotionIsAppliedEvent);
      else
        this.Fsm.Event(this.rootMotionIsNotAppliedEvent);
    }
  }
}
