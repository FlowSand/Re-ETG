using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Returns the culling of this Animator component. Optionnaly sends events.\nIf true ('AlwaysAnimate'): always animate the entire character. Object is animated even when offscreen.\nIf False ('BasedOnRenderers') animation is disabled when renderers are not visible.")]
  [ActionCategory(ActionCategory.Animator)]
  public class GetAnimatorCullingMode : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
    [CheckForComponent(typeof (Animator))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("If true, always animate the entire character, else animation is disabled when renderers are not visible")]
    [ActionSection("Results")]
    [RequiredField]
    public FsmBool alwaysAnimate;
    [HutongGames.PlayMaker.Tooltip("Event send if culling mode is 'AlwaysAnimate'")]
    public FsmEvent alwaysAnimateEvent;
    [HutongGames.PlayMaker.Tooltip("Event send if culling mode is 'BasedOnRenders'")]
    public FsmEvent basedOnRenderersEvent;
    private Animator _animator;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.alwaysAnimate = (FsmBool) null;
      this.alwaysAnimateEvent = (FsmEvent) null;
      this.basedOnRenderersEvent = (FsmEvent) null;
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
          this.DoCheckCulling();
          this.Finish();
        }
      }
    }

    private void DoCheckCulling()
    {
      if ((Object) this._animator == (Object) null)
        return;
      bool flag = this._animator.cullingMode == AnimatorCullingMode.AlwaysAnimate;
      this.alwaysAnimate.Value = flag;
      if (flag)
        this.Fsm.Event(this.alwaysAnimateEvent);
      else
        this.Fsm.Event(this.basedOnRenderersEvent);
    }
  }
}
