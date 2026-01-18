using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Animation)]
  [HutongGames.PlayMaker.Tooltip("Sets the current Time of an Animation, Normalize time means 0 (start) to 1 (end); useful if you don't care about the exact time. Check Every Frame to update the time continuosly.")]
  public class SetAnimationTime : BaseAnimationAction
  {
    [CheckForComponent(typeof (Animation))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [UIHint(UIHint.Animation)]
    [RequiredField]
    public FsmString animName;
    public FsmFloat time;
    public bool normalized;
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.animName = (FsmString) null;
      this.time = (FsmFloat) null;
      this.normalized = false;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetAnimationTime(this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner);
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      this.DoSetAnimationTime(this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner);
    }

    private void DoSetAnimationTime(GameObject go)
    {
      if (!this.UpdateCache(go))
        return;
      this.animation.Play(this.animName.Value);
      AnimationState animationState = this.animation[this.animName.Value];
      if ((TrackedReference) animationState == (TrackedReference) null)
      {
        this.LogWarning("Missing animation: " + this.animName.Value);
      }
      else
      {
        if (this.normalized)
          animationState.normalizedTime = this.time.Value;
        else
          animationState.time = this.time.Value;
        if (!this.everyFrame)
          return;
        animationState.speed = 0.0f;
      }
    }
  }
}
