// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.PlayBraveAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".Brave")]
  [HutongGames.PlayMaker.Tooltip("Plays an animation on the specified object.")]
  public class PlayBraveAnimation : FsmStateAction
  {
    public FsmOwnerDefault GameObject;
    [HutongGames.PlayMaker.Tooltip("Name of the animation to play.")]
    public FsmString animName;
    [HutongGames.PlayMaker.Tooltip("How to play the animation.")]
    public PlayBraveAnimation.PlayMode mode;
    [HutongGames.PlayMaker.Tooltip("How long to play the animation for.")]
    public FsmFloat duration;
    [HutongGames.PlayMaker.Tooltip("If the animation is already playing, don't trigger it again.")]
    public FsmBool dontPlayIfPlaying;
    [HutongGames.PlayMaker.Tooltip("What animation to play next.")]
    public PlayBraveAnimation.NextMode next;
    [HutongGames.PlayMaker.Tooltip("The next animation to play (used only for UntilFinishedThenNext).")]
    public FsmString nextAnimName;
    [HutongGames.PlayMaker.Tooltip("Time to wait after the animation before continuing to the next action; 0 continues immediately.")]
    public FsmFloat waitTime;
    public FsmBool playOnOtherTalkDoerInRoom;
    private float m_timer;

    public override void Reset()
    {
      this.GameObject = (FsmOwnerDefault) null;
      this.animName = (FsmString) string.Empty;
      this.mode = PlayBraveAnimation.PlayMode.UntilCancelled;
      this.nextAnimName = (FsmString) string.Empty;
      this.waitTime = (FsmFloat) 0.0f;
    }

    public override string ErrorCheck()
    {
      string str = string.Empty;
      UnityEngine.GameObject gameObject = this.GameObject.OwnerOption != OwnerDefaultOption.UseOwner ? this.GameObject.GameObject.Value : this.Owner;
      if ((bool) (Object) gameObject)
      {
        tk2dSpriteAnimator component1 = gameObject.GetComponent<tk2dSpriteAnimator>();
        AIAnimator component2 = gameObject.GetComponent<AIAnimator>();
        if (!(bool) (Object) component1 && !(bool) (Object) component2)
          return "Requires a 2D Toolkit animator or an AI Animator.\n";
        if ((bool) (Object) component2)
        {
          if (!component2.HasDirectionalAnimation(this.animName.Value))
            str = $"{str}Unknown animation {this.animName.Value}.\n";
          if (this.UsesNextAnim && !component2.HasDirectionalAnimation(this.nextAnimName.Value))
            str = $"{str}Unknown animation {this.nextAnimName.Value}.\n";
        }
        else if ((bool) (Object) component1)
        {
          if (component1.GetClipByName(this.animName.Value) == null)
            str = $"{str}Unknown animation {this.animName.Value}.\n";
          if (this.UsesNextAnim && component1.GetClipByName(this.nextAnimName.Value) == null)
            str = $"{str}Unknown animation {this.nextAnimName.Value}.\n";
        }
      }
      else if (!this.GameObject.GameObject.UseVariable)
        return "No object specified";
      return str;
    }

    public override void OnEnter()
    {
      UnityEngine.GameObject gameObject = this.Fsm.GetOwnerDefaultTarget(this.GameObject);
      if (this.playOnOtherTalkDoerInRoom.Value)
      {
        TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
        for (int index = 0; index < StaticReferenceManager.AllNpcs.Count; ++index)
        {
          if (StaticReferenceManager.AllNpcs[index].ParentRoom == component.ParentRoom && (Object) StaticReferenceManager.AllNpcs[index] != (Object) component)
          {
            gameObject = StaticReferenceManager.AllNpcs[index].gameObject;
            break;
          }
        }
      }
      tk2dSpriteAnimator component1 = gameObject.GetComponent<tk2dSpriteAnimator>();
      AIAnimator component2 = gameObject.GetComponent<AIAnimator>();
      string str = !((Object) component1 != (Object) null) || component1.CurrentClip == null ? string.Empty : component1.CurrentClip.name;
      if (!this.dontPlayIfPlaying.Value || !(str == this.animName.Value))
      {
        if (this.mode == PlayBraveAnimation.PlayMode.UntilCancelled)
        {
          if ((bool) (Object) component2)
          {
            bool flag = true;
            if ((bool) (Object) component2.talkDoer && this.animName.Value == "idle" && component2.talkDoer.IsPlayingZombieAnimation)
              flag = false;
            if (flag)
              component2.PlayUntilCancelled(this.animName.Value);
          }
          else
            component1.Play(this.animName.Value);
        }
        else if (this.mode == PlayBraveAnimation.PlayMode.Duration)
        {
          if ((bool) (Object) component2)
            component2.PlayForDuration(this.animName.Value, this.duration.Value);
          else if (this.next == PlayBraveAnimation.NextMode.ReturnToPrevious)
            component1.PlayForDuration(this.animName.Value, this.duration.Value);
          else if (this.next == PlayBraveAnimation.NextMode.NewAnimation)
            component1.PlayForDuration(this.animName.Value, this.duration.Value, this.nextAnimName.Value);
        }
        else if (this.mode == PlayBraveAnimation.PlayMode.UntilFinished)
        {
          if ((bool) (Object) component2)
            component2.PlayUntilFinished(this.animName.Value);
          else if (this.next == PlayBraveAnimation.NextMode.ReturnToPrevious)
            component1.PlayForDuration(this.animName.Value, -1f);
          else if (this.next == PlayBraveAnimation.NextMode.NewAnimation)
            component1.PlayForDuration(this.animName.Value, -1f, this.nextAnimName.Value);
        }
      }
      if ((double) this.waitTime.Value > 0.0)
        this.m_timer = this.waitTime.Value;
      else
        this.Finish();
    }

    public override void OnUpdate()
    {
      if ((double) this.m_timer <= 0.0)
        return;
      this.m_timer -= BraveTime.DeltaTime;
      if ((double) this.m_timer > 0.0)
        return;
      this.Finish();
    }

    private bool UsesNextAnim => this.next == PlayBraveAnimation.NextMode.NewAnimation;

    public enum PlayMode
    {
      UntilCancelled,
      Duration,
      UntilFinished,
    }

    public enum NextMode
    {
      ReturnToPrevious,
      NewAnimation,
    }
  }
}
