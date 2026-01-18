using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Animation)]
  [HutongGames.PlayMaker.Tooltip("Rewinds the named animation.")]
  public class RewindAnimation : BaseAnimationAction
  {
    [CheckForComponent(typeof (Animation))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Animation)]
    public FsmString animName;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.animName = (FsmString) null;
    }

    public override void OnEnter()
    {
      this.DoRewindAnimation();
      this.Finish();
    }

    private void DoRewindAnimation()
    {
      if (string.IsNullOrEmpty(this.animName.Value) || !this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      this.animation.Rewind(this.animName.Value);
    }
  }
}
