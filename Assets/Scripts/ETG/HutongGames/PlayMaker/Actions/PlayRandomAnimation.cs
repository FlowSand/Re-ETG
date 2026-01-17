// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.PlayRandomAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Plays a Random Animation on a Game Object. You can set the relative weight of each animation to control how often they are selected.")]
[ActionCategory(ActionCategory.Animation)]
public class PlayRandomAnimation : BaseAnimationAction
{
  [RequiredField]
  [CheckForComponent(typeof (Animation))]
  [HutongGames.PlayMaker.Tooltip("Game Object to play the animation on.")]
  public FsmOwnerDefault gameObject;
  [UIHint(UIHint.Animation)]
  [CompoundArray("Animations", "Animation", "Weight")]
  public FsmString[] animations;
  [HasFloatSlider(0.0f, 1f)]
  public FsmFloat[] weights;
  [HutongGames.PlayMaker.Tooltip("How to treat previously playing animations.")]
  public UnityEngine.PlayMode playMode;
  [HutongGames.PlayMaker.Tooltip("Time taken to blend to this animation.")]
  [HasFloatSlider(0.0f, 5f)]
  public FsmFloat blendTime;
  [HutongGames.PlayMaker.Tooltip("Event to send when the animation is finished playing. NOTE: Not sent with Loop or PingPong wrap modes!")]
  public FsmEvent finishEvent;
  [HutongGames.PlayMaker.Tooltip("Event to send when the animation loops. If you want to send this event to another FSM use Set Event Target. NOTE: This event is only sent with Loop and PingPong wrap modes.")]
  public FsmEvent loopEvent;
  [HutongGames.PlayMaker.Tooltip("Stop playing the animation when this state is exited.")]
  public bool stopOnExit;
  private AnimationState anim;
  private float prevAnimtTime;

  public override void Reset()
  {
    this.gameObject = (FsmOwnerDefault) null;
    this.animations = new FsmString[0];
    this.weights = new FsmFloat[0];
    this.playMode = UnityEngine.PlayMode.StopAll;
    this.blendTime = (FsmFloat) 0.3f;
    this.finishEvent = (FsmEvent) null;
    this.loopEvent = (FsmEvent) null;
    this.stopOnExit = false;
  }

  public override void OnEnter() => this.DoPlayRandomAnimation();

  private void DoPlayRandomAnimation()
  {
    if (this.animations.Length <= 0)
      return;
    int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
    if (randomWeightedIndex == -1)
      return;
    this.DoPlayAnimation(this.animations[randomWeightedIndex].Value);
  }

  private void DoPlayAnimation(string animName)
  {
    if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
      this.Finish();
    else if (string.IsNullOrEmpty(animName))
    {
      this.LogWarning("Missing animName!");
      this.Finish();
    }
    else
    {
      this.anim = this.animation[animName];
      if ((TrackedReference) this.anim == (TrackedReference) null)
      {
        this.LogWarning("Missing animation: " + animName);
        this.Finish();
      }
      else
      {
        float fadeLength = this.blendTime.Value;
        if ((double) fadeLength < 1.0 / 1000.0)
          this.animation.Play(animName, this.playMode);
        else
          this.animation.CrossFade(animName, fadeLength, this.playMode);
        this.prevAnimtTime = this.anim.time;
      }
    }
  }

  public override void OnUpdate()
  {
    if ((Object) this.Fsm.GetOwnerDefaultTarget(this.gameObject) == (Object) null || (TrackedReference) this.anim == (TrackedReference) null)
      return;
    if (!this.anim.enabled || this.anim.wrapMode == WrapMode.ClampForever && (double) this.anim.time > (double) this.anim.length)
    {
      this.Fsm.Event(this.finishEvent);
      this.Finish();
    }
    if (this.anim.wrapMode == WrapMode.ClampForever || (double) this.anim.time <= (double) this.anim.length || (double) this.prevAnimtTime >= (double) this.anim.length)
      return;
    this.Fsm.Event(this.loopEvent);
  }

  public override void OnExit()
  {
    if (!this.stopOnExit)
      return;
    this.StopAnimation();
  }

  private void StopAnimation()
  {
    if (!((Object) this.animation != (Object) null))
      return;
    this.animation.Stop(this.anim.name);
  }
}
