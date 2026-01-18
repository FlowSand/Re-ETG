using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Set the Wrap Mode, Blend Mode, Layer and Speed of an Animation.\nNOTE: Settings are applied once, on entering the state, NOT continuously. To dynamically control an animation's settings, use Set Animation Speede etc.")]
  [ActionCategory(ActionCategory.Animation)]
  public class AnimationSettings : BaseAnimationAction
  {
    [CheckForComponent(typeof (Animation))]
    [HutongGames.PlayMaker.Tooltip("A GameObject with an Animation Component.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Animation)]
    [HutongGames.PlayMaker.Tooltip("The name of the animation.")]
    [RequiredField]
    public FsmString animName;
    [HutongGames.PlayMaker.Tooltip("The behavior of the animation when it wraps.")]
    public WrapMode wrapMode;
    [HutongGames.PlayMaker.Tooltip("How the animation is blended with other animations on the Game Object.")]
    public AnimationBlendMode blendMode;
    [HutongGames.PlayMaker.Tooltip("The speed of the animation. 1 = normal; 2 = double speed...")]
    [HasFloatSlider(0.0f, 5f)]
    public FsmFloat speed;
    [HutongGames.PlayMaker.Tooltip("The animation layer")]
    public FsmInt layer;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.animName = (FsmString) null;
      this.wrapMode = WrapMode.Loop;
      this.blendMode = AnimationBlendMode.Blend;
      this.speed = (FsmFloat) 1f;
      this.layer = (FsmInt) 0;
    }

    public override void OnEnter()
    {
      this.DoAnimationSettings();
      this.Finish();
    }

    private void DoAnimationSettings()
    {
      if (string.IsNullOrEmpty(this.animName.Value) || !this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      AnimationState animationState = this.animation[this.animName.Value];
      if ((TrackedReference) animationState == (TrackedReference) null)
      {
        this.LogWarning("Missing animation: " + this.animName.Value);
      }
      else
      {
        animationState.wrapMode = this.wrapMode;
        animationState.blendMode = this.blendMode;
        if (!this.layer.IsNone)
          animationState.layer = this.layer.Value;
        if (this.speed.IsNone)
          return;
        animationState.speed = this.speed.Value;
      }
    }
  }
}
