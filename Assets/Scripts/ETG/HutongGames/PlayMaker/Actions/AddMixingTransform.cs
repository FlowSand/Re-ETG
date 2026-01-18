using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Play an animation on a subset of the hierarchy. E.g., A waving animation on the upper body.")]
  [ActionCategory(ActionCategory.Animation)]
  public class AddMixingTransform : BaseAnimationAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject playing the animation.")]
    [RequiredField]
    [CheckForComponent(typeof (Animation))]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The name of the animation to mix. NOTE: The animation should already be added to the Animation Component on the GameObject.")]
    public FsmString animationName;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The mixing transform. E.g., root/upper_body/left_shoulder")]
    public FsmString transform;
    [HutongGames.PlayMaker.Tooltip("If recursive is true all children of the mix transform will also be animated.")]
    public FsmBool recursive;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.animationName = (FsmString) string.Empty;
      this.transform = (FsmString) string.Empty;
      this.recursive = (FsmBool) true;
    }

    public override void OnEnter()
    {
      this.DoAddMixingTransform();
      this.Finish();
    }

    private void DoAddMixingTransform()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if (!this.UpdateCache(ownerDefaultTarget))
        return;
      AnimationState animationState = this.animation[this.animationName.Value];
      if ((TrackedReference) animationState == (TrackedReference) null)
        return;
      Transform mix = ownerDefaultTarget.transform.Find(this.transform.Value);
      animationState.AddMixingTransform(mix, this.recursive.Value);
    }
  }
}
