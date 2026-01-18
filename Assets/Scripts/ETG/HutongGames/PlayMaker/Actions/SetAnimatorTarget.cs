using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets an AvatarTarget and a targetNormalizedTime for the current state")]
  [ActionCategory(ActionCategory.Animator)]
  public class SetAnimatorTarget : FsmStateAction
  {
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The target.")]
    [CheckForComponent(typeof (Animator))]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The avatar target")]
    public AvatarTarget avatarTarget;
    [HutongGames.PlayMaker.Tooltip("The current state Time that is queried")]
    public FsmFloat targetNormalizedTime;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame during OnAnimatorMove. Useful when changing over time.")]
    public bool everyFrame;
    private Animator _animator;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.avatarTarget = AvatarTarget.Body;
      this.targetNormalizedTime = (FsmFloat) null;
      this.everyFrame = false;
    }

    public override void OnPreprocess() => this.Fsm.HandleAnimatorMove = true;

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
          this.SetTarget();
          if (this.everyFrame)
            return;
          this.Finish();
        }
      }
    }

    public override void DoAnimatorMove() => this.SetTarget();

    private void SetTarget()
    {
      if (!((Object) this._animator != (Object) null))
        return;
      this._animator.SetTarget(this.avatarTarget, this.targetNormalizedTime.Value);
    }
  }
}
