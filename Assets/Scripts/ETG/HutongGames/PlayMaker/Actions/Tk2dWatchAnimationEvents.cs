using System;
using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Receive animation events and animation complete event of the current animation playing. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
  [ActionCategory("2D Toolkit/SpriteAnimator")]
  public class Tk2dWatchAnimationEvents : FsmStateAction
  {
    [CheckForComponent(typeof (tk2dSpriteAnimator))]
    [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Trigger event defined in the clip. The event holds the following triggers infos: the eventInt, eventInfo and eventFloat properties")]
    public FsmEvent animationTriggerEvent;
    [HutongGames.PlayMaker.Tooltip("Animation complete event. The event holds the clipId reference")]
    public FsmEvent animationCompleteEvent;
    private tk2dSpriteAnimator _sprite;

    private void _getSprite()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((UnityEngine.Object) ownerDefaultTarget == (UnityEngine.Object) null)
        return;
      this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
    }

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.animationTriggerEvent = (FsmEvent) null;
      this.animationCompleteEvent = (FsmEvent) null;
    }

    public override void OnEnter()
    {
      this._getSprite();
      this.DoWatchAnimationWithEvents();
    }

    private void DoWatchAnimationWithEvents()
    {
      if ((UnityEngine.Object) this._sprite == (UnityEngine.Object) null)
      {
        this.LogWarning("Missing tk2dSpriteAnimator component");
      }
      else
      {
        if (this.animationTriggerEvent != null)
          this._sprite.AnimationEventTriggered = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventDelegate);
        if (this.animationCompleteEvent == null)
          return;
        this._sprite.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
      }
    }

    private void AnimationEventDelegate(
      tk2dSpriteAnimator sprite,
      tk2dSpriteAnimationClip clip,
      int frameNum)
    {
      tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
      Fsm.EventData.IntData = frame.eventInt;
      Fsm.EventData.StringData = frame.eventInfo;
      Fsm.EventData.FloatData = frame.eventFloat;
      this.Fsm.Event(this.animationTriggerEvent);
    }

    private void AnimationCompleteDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
    {
      int num = -1;
      tk2dSpriteAnimationClip[] clips = !((UnityEngine.Object) sprite.Library != (UnityEngine.Object) null) ? (tk2dSpriteAnimationClip[]) null : sprite.Library.clips;
      if (clips != null)
      {
        for (int index = 0; index < clips.Length; ++index)
        {
          if (clips[index] == clip)
          {
            num = index;
            break;
          }
        }
      }
      Fsm.EventData.IntData = num;
      this.Fsm.Event(this.animationCompleteEvent);
    }
  }
}
