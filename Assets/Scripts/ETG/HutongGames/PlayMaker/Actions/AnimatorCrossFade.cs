using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Create a dynamic transition between the current state and the destination state.Both state as to be on the same layer. note: You cannot change the current state on a synchronized layer, you need to change it on the referenced layer.")]
  [ActionCategory(ActionCategory.Animator)]
  public class AnimatorCrossFade : FsmStateAction
  {
    [CheckForComponent(typeof (Animator))]
    [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The name of the state that will be played.")]
    public FsmString stateName;
    [HutongGames.PlayMaker.Tooltip("The duration of the transition. Value is in source state normalized time.")]
    public FsmFloat transitionDuration;
    [HutongGames.PlayMaker.Tooltip("Layer index containing the destination state. Leave to none to ignore")]
    public FsmInt layer;
    [HutongGames.PlayMaker.Tooltip("Start time of the current destination state. Value is in source state normalized time, should be between 0 and 1.")]
    public FsmFloat normalizedTime;
    private Animator _animator;
    private int _paramID;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.stateName = (FsmString) null;
      this.transitionDuration = (FsmFloat) 1f;
      FsmInt fsmInt = new FsmInt();
      fsmInt.UseVariable = true;
      this.layer = fsmInt;
      FsmFloat fsmFloat = new FsmFloat();
      fsmFloat.UseVariable = true;
      this.normalizedTime = fsmFloat;
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
        if ((Object) this._animator != (Object) null)
          this._animator.CrossFade(this.stateName.Value, this.transitionDuration.Value, !this.layer.IsNone ? this.layer.Value : -1, !this.normalizedTime.IsNone ? this.normalizedTime.Value : float.NegativeInfinity);
        this.Finish();
      }
    }
  }
}
