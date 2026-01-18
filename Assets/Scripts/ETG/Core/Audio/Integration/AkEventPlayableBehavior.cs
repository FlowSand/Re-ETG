// Decompiled with JetBrains decompiler
// Type: AkEventPlayableBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Playables;

#nullable disable

public class AkEventPlayableBehavior : PlayableBehaviour
  {
    public static int scrubPlaybackLengthMs = 100;
    public AK.Wwise.Event akEvent;
    public float akEventMaxDuration = -1f;
    public float akEventMinDuration = -1f;
    public float blendInDuration;
    public float blendOutDuration;
    public float easeInDuration;
    public float easeOutDuration;
    public GameObject eventObject;
    public bool eventShouldRetrigger;
    public WwiseEventTracker eventTracker;
    public float lastEffectiveWeight = 1f;
    public bool overrideTrackEmittorObject;
    public uint requiredActions;

    public override void PrepareFrame(Playable playable, FrameData info)
    {
      if (this.eventTracker == null)
        return;
      if (info.evaluationType == FrameData.EvaluationType.Evaluate && Application.isPlaying && this.ShouldPlay(playable))
      {
        if (!this.eventTracker.eventIsPlaying)
        {
          this.requiredActions |= 1U;
          this.requiredActions |= 8U;
          this.checkForFadeIn((float) playable.GetTime<Playable>());
          this.checkForFadeOut(playable);
        }
        this.requiredActions |= 16U /*0x10*/;
      }
      else
      {
        if (!this.eventTracker.eventIsPlaying && ((int) this.requiredActions & 1) == 0)
        {
          this.requiredActions |= 2U;
          this.checkForFadeIn((float) playable.GetTime<Playable>());
        }
        this.checkForFadeOut(playable);
      }
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
      if (this.akEvent == null || !this.ShouldPlay(playable))
        return;
      this.requiredActions |= 1U;
      if (info.evaluationType == FrameData.EvaluationType.Evaluate && Application.isPlaying)
      {
        this.requiredActions |= 8U;
        this.checkForFadeIn((float) playable.GetTime<Playable>());
        this.checkForFadeOut(playable);
      }
      else
      {
        if ((double) this.getProportionalTime(playable) > 0.05000000074505806)
          this.requiredActions |= 16U /*0x10*/;
        this.checkForFadeIn((float) playable.GetTime<Playable>());
        this.checkForFadeOut(playable);
      }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
      if (!((Object) this.eventObject != (Object) null))
        return;
      this.stopEvent();
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
      if (!this.overrideTrackEmittorObject)
      {
        GameObject gameObject = playerData as GameObject;
        if ((Object) gameObject != (Object) null)
          this.eventObject = gameObject;
      }
      if ((Object) this.eventObject != (Object) null)
      {
        float time1 = (float) playable.GetTime<Playable>();
        if (this.actionIsRequired(AkEventPlayableBehavior.AkPlayableAction.Playback))
          this.playEvent();
        if (this.eventShouldRetrigger && this.actionIsRequired(AkEventPlayableBehavior.AkPlayableAction.Retrigger))
          this.retriggerEvent(playable);
        if (this.actionIsRequired(AkEventPlayableBehavior.AkPlayableAction.Stop))
          this.akEvent.Stop(this.eventObject);
        if (this.actionIsRequired(AkEventPlayableBehavior.AkPlayableAction.DelayedStop))
          this.stopEvent(AkEventPlayableBehavior.scrubPlaybackLengthMs);
        if (this.actionIsRequired(AkEventPlayableBehavior.AkPlayableAction.Seek))
        {
          double time2 = (double) this.seekToTime(playable);
        }
        if (this.actionIsRequired(AkEventPlayableBehavior.AkPlayableAction.FadeIn))
          this.triggerFadeIn(time1);
        if (this.actionIsRequired(AkEventPlayableBehavior.AkPlayableAction.FadeOut))
          this.triggerFadeOut((float) (playable.GetDuration<Playable>() - playable.GetTime<Playable>()));
      }
      this.requiredActions = 0U;
    }

    private bool actionIsRequired(
      AkEventPlayableBehavior.AkPlayableAction actionType)
    {
      return ((AkEventPlayableBehavior.AkPlayableAction) this.requiredActions & actionType) != AkEventPlayableBehavior.AkPlayableAction.None;
    }

    private bool ShouldPlay(Playable playable)
    {
      if (this.eventTracker == null)
        return false;
      if ((double) this.akEventMaxDuration == (double) this.akEventMinDuration && (double) this.akEventMinDuration != -1.0)
        return playable.GetTime<Playable>() < (double) this.akEventMaxDuration || this.eventShouldRetrigger;
      float num1 = (float) playable.GetTime<Playable>() - this.eventTracker.previousEventStartTime;
      float currentDuration = this.eventTracker.currentDuration;
      float num2 = (double) currentDuration != -1.0 ? currentDuration : (float) playable.GetDuration<Playable>();
      return (double) num1 < (double) num2 || this.eventShouldRetrigger;
    }

    private bool fadeInRequired(float currentClipTime)
    {
      float num1 = this.blendInDuration - currentClipTime;
      float num2 = this.easeInDuration - currentClipTime;
      return (double) num1 > 0.0 || (double) num2 > 0.0;
    }

    private void checkForFadeIn(float currentClipTime)
    {
      if (!this.fadeInRequired(currentClipTime))
        return;
      this.requiredActions |= 32U /*0x20*/;
    }

    private void checkForFadeInImmediate(float currentClipTime)
    {
      if (!this.fadeInRequired(currentClipTime))
        return;
      this.triggerFadeIn(currentClipTime);
    }

    private bool fadeOutRequired(Playable playable)
    {
      float num1 = (float) (playable.GetDuration<Playable>() - playable.GetTime<Playable>());
      float num2 = this.blendOutDuration - num1;
      float num3 = this.easeOutDuration - num1;
      return (double) num2 >= 0.0 || (double) num3 >= 0.0;
    }

    private void checkForFadeOutImmediate(Playable playable)
    {
      if (this.eventTracker == null || this.eventTracker.fadeoutTriggered || !this.fadeOutRequired(playable))
        return;
      this.triggerFadeOut((float) (playable.GetDuration<Playable>() - playable.GetTime<Playable>()));
    }

    private void checkForFadeOut(Playable playable)
    {
      if (this.eventTracker == null || this.eventTracker.fadeoutTriggered || !this.fadeOutRequired(playable))
        return;
      this.requiredActions |= 64U /*0x40*/;
    }

    protected void triggerFadeIn(float currentClipTime)
    {
      if (!((Object) this.eventObject != (Object) null) || this.akEvent == null)
        return;
      float num = Mathf.Max(this.easeInDuration - currentClipTime, this.blendInDuration - currentClipTime);
      if ((double) num <= 0.0)
        return;
      this.akEvent.ExecuteAction(this.eventObject, AkActionOnEventType.AkActionOnEventType_Pause, 0, AkCurveInterpolation.AkCurveInterpolation_Linear);
      this.akEvent.ExecuteAction(this.eventObject, AkActionOnEventType.AkActionOnEventType_Resume, (int) ((double) num * 1000.0), AkCurveInterpolation.AkCurveInterpolation_Linear);
    }

    protected void triggerFadeOut(float fadeDuration)
    {
      if (!((Object) this.eventObject != (Object) null) || this.akEvent == null)
        return;
      if (this.eventTracker != null)
        this.eventTracker.fadeoutTriggered = true;
      this.akEvent.ExecuteAction(this.eventObject, AkActionOnEventType.AkActionOnEventType_Stop, (int) ((double) fadeDuration * 1000.0), AkCurveInterpolation.AkCurveInterpolation_Linear);
    }

    protected void stopEvent(int transition = 0)
    {
      if (!((Object) this.eventObject != (Object) null) || this.akEvent == null || !this.eventTracker.eventIsPlaying)
        return;
      this.akEvent.Stop(this.eventObject, transition);
      if (this.eventTracker == null)
        return;
      this.eventTracker.eventIsPlaying = false;
    }

    protected void playEvent()
    {
      if (!((Object) this.eventObject != (Object) null) || this.akEvent == null || this.eventTracker == null)
        return;
      this.eventTracker.playingID = this.akEvent.Post(this.eventObject, 9U, new AkCallbackManager.EventCallback(this.eventTracker.CallbackHandler));
      if (this.eventTracker.playingID == 0U)
        return;
      this.eventTracker.eventIsPlaying = true;
      this.eventTracker.currentDurationProportion = 1f;
      this.eventTracker.previousEventStartTime = 0.0f;
    }

    protected void retriggerEvent(Playable playable)
    {
      if (!((Object) this.eventObject != (Object) null) || this.akEvent == null || this.eventTracker == null)
        return;
      this.eventTracker.playingID = this.akEvent.Post(this.eventObject, 9U, new AkCallbackManager.EventCallback(this.eventTracker.CallbackHandler));
      if (this.eventTracker.playingID == 0U)
        return;
      this.eventTracker.eventIsPlaying = true;
      this.eventTracker.currentDurationProportion = this.seekToTime(playable);
      this.eventTracker.previousEventStartTime = (float) playable.GetTime<Playable>();
    }

    protected float getProportionalTime(Playable playable)
    {
      if (this.eventTracker == null)
        return 0.0f;
      if ((double) this.akEventMaxDuration == (double) this.akEventMinDuration && (double) this.akEventMinDuration != -1.0)
        return (float) playable.GetTime<Playable>() % this.akEventMaxDuration / this.akEventMaxDuration;
      float num1 = (float) playable.GetTime<Playable>() - this.eventTracker.previousEventStartTime;
      float currentDuration = this.eventTracker.currentDuration;
      float num2 = (double) currentDuration != -1.0 ? currentDuration : (float) playable.GetDuration<Playable>();
      return num1 % num2 / num2;
    }

    protected float seekToTime(Playable playable)
    {
      if ((Object) this.eventObject != (Object) null && this.akEvent != null)
      {
        float proportionalTime = this.getProportionalTime(playable);
        if ((double) proportionalTime < 1.0)
        {
          int num = (int) AkSoundEngine.SeekOnEvent((uint) this.akEvent.ID, this.eventObject, proportionalTime);
          return 1f - proportionalTime;
        }
      }
      return 1f;
    }

    public enum AkPlayableAction
    {
      None = 0,
      Playback = 1,
      Retrigger = 2,
      Stop = 4,
      DelayedStop = 8,
      Seek = 16, // 0x00000010
      FadeIn = 32, // 0x00000020
      FadeOut = 64, // 0x00000040
    }
  }

