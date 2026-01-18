using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Animator)]
  [HutongGames.PlayMaker.Tooltip("Returns if additionnal layers affects the mass center")]
  public class GetAnimatorLayersAffectMassCenter : FsmStateAction
  {
    [CheckForComponent(typeof (Animator))]
    [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    [ActionSection("Results")]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("If true, additionnal layers affects the mass center")]
    public FsmBool affectMassCenter;
    [HutongGames.PlayMaker.Tooltip("Event send if additionnal layers affects the mass center")]
    public FsmEvent affectMassCenterEvent;
    [HutongGames.PlayMaker.Tooltip("Event send if additionnal layers do no affects the mass center")]
    public FsmEvent doNotAffectMassCenterEvent;
    private Animator _animator;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.affectMassCenter = (FsmBool) null;
      this.affectMassCenterEvent = (FsmEvent) null;
      this.doNotAffectMassCenterEvent = (FsmEvent) null;
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
          this.CheckAffectMassCenter();
          this.Finish();
        }
      }
    }

    private void CheckAffectMassCenter()
    {
      if ((Object) this._animator == (Object) null)
        return;
      bool affectMassCenter = this._animator.layersAffectMassCenter;
      this.affectMassCenter.Value = affectMassCenter;
      if (affectMassCenter)
        this.Fsm.Event(this.affectMassCenterEvent);
      else
        this.Fsm.Event(this.doNotAffectMassCenterEvent);
    }
  }
}
