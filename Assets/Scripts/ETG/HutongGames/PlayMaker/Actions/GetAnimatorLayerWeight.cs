using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the layer's current weight")]
  [ActionCategory(ActionCategory.Animator)]
  public class GetAnimatorLayerWeight : FsmStateActionAnimatorBase
  {
    [HutongGames.PlayMaker.Tooltip("The target.")]
    [CheckForComponent(typeof (Animator))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The layer's index")]
    [RequiredField]
    public FsmInt layerIndex;
    [UIHint(UIHint.Variable)]
    [RequiredField]
    [ActionSection("Results")]
    [HutongGames.PlayMaker.Tooltip("The layer's current weight")]
    public FsmFloat layerWeight;
    private Animator _animator;

    public override void Reset()
    {
      base.Reset();
      this.gameObject = (FsmOwnerDefault) null;
      this.layerIndex = (FsmInt) null;
      this.layerWeight = (FsmFloat) null;
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
          this.GetLayerWeight();
          if (this.everyFrame)
            return;
          this.Finish();
        }
      }
    }

    public override void OnActionUpdate() => this.GetLayerWeight();

    private void GetLayerWeight()
    {
      if (!((Object) this._animator != (Object) null))
        return;
      this.layerWeight.Value = this._animator.GetLayerWeight(this.layerIndex.Value);
    }
  }
}
