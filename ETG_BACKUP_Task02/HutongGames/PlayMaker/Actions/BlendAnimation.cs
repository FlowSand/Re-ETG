// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.BlendAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Animation)]
[HutongGames.PlayMaker.Tooltip("Blends an Animation towards a Target Weight over a specified Time.\nOptionally sends an Event when finished.")]
public class BlendAnimation : BaseAnimationAction
{
  [HutongGames.PlayMaker.Tooltip("The GameObject to animate.")]
  [CheckForComponent(typeof (Animation))]
  [RequiredField]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.Animation)]
  [HutongGames.PlayMaker.Tooltip("The name of the animation to blend.")]
  [RequiredField]
  public FsmString animName;
  [HasFloatSlider(0.0f, 1f)]
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("Target weight to blend to.")]
  public FsmFloat targetWeight;
  [HutongGames.PlayMaker.Tooltip("How long should the blend take.")]
  [HasFloatSlider(0.0f, 5f)]
  [RequiredField]
  public FsmFloat time;
  [HutongGames.PlayMaker.Tooltip("Event to send when the blend has finished.")]
  public FsmEvent finishEvent;
  private DelayedEvent delayedFinishEvent;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.animName = (FsmString) null;
    this.targetWeight = (FsmFloat) 1f;
    this.time = (FsmFloat) 0.3f;
    this.finishEvent = (FsmEvent) null;
  }

  public override void OnEnter()
  {
    this.DoBlendAnimation(this.gameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.gameObject.GameObject.Value : this.Owner);
  }

  public override void OnUpdate()
  {
    if (!DelayedEvent.WasSent(this.delayedFinishEvent))
      return;
    this.Finish();
  }

  private void DoBlendAnimation(GameObject go)
  {
    if ((Object) go == (Object) null)
      return;
    Animation component = go.GetComponent<Animation>();
    if ((Object) component == (Object) null)
    {
      this.LogWarning("Missing Animation component on GameObject: " + go.name);
      this.Finish();
    }
    else
    {
      AnimationState animationState = component[this.animName.Value];
      if ((TrackedReference) animationState == (TrackedReference) null)
      {
        this.LogWarning("Missing animation: " + this.animName.Value);
        this.Finish();
      }
      else
      {
        float fadeLength = this.time.Value;
        component.Blend(this.animName.Value, this.targetWeight.Value, fadeLength);
        if (this.finishEvent != null)
          this.delayedFinishEvent = this.Fsm.DelayedEvent(this.finishEvent, animationState.length);
        else
          this.Finish();
      }
    }
  }
}
