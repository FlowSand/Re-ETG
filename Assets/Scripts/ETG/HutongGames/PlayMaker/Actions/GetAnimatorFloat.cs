using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the value of a float parameter")]
  [ActionCategory(ActionCategory.Animator)]
  public class GetAnimatorFloat : FsmStateActionAnimatorBase
  {
    [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
    [CheckForComponent(typeof (Animator))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The animator parameter")]
    [UIHint(UIHint.AnimatorFloat)]
    [RequiredField]
    public FsmString parameter;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("The float value of the animator parameter")]
    [ActionSection("Results")]
    [RequiredField]
    public FsmFloat result;
    private Animator _animator;
    private int _paramID;

    public override void Reset()
    {
      base.Reset();
      this.gameObject = (FsmOwnerDefault) null;
      this.parameter = (FsmString) null;
      this.result = (FsmFloat) null;
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
          this._paramID = Animator.StringToHash(this.parameter.Value);
          this.GetParameter();
          if (this.everyFrame)
            return;
          this.Finish();
        }
      }
    }

    public override void OnActionUpdate() => this.GetParameter();

    private void GetParameter()
    {
      if (!((Object) this._animator != (Object) null))
        return;
      this.result.Value = this._animator.GetFloat(this._paramID);
    }
  }
}
