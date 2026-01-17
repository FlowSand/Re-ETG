// Decompiled with JetBrains decompiler
// Type: AudioAnimatorListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
public class AudioAnimatorListener : BraveBehaviour
{
  public ActorAudioEvent[] animationAudioEvents;

  private void Start()
  {
    this.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent);
    if (this.spriteAnimator.CurrentClip == null)
      return;
    this.HandleAnimationEvent(this.spriteAnimator, this.spriteAnimator.CurrentClip, 0);
  }

  protected void HandleAnimationEvent(
    tk2dSpriteAnimator animator,
    tk2dSpriteAnimationClip clip,
    int frameNo)
  {
    tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNo);
    for (int index = 0; index < this.animationAudioEvents.Length; ++index)
    {
      if (this.animationAudioEvents[index].eventTag == frame.eventInfo)
      {
        int num = (int) AkSoundEngine.PostEvent(this.animationAudioEvents[index].eventName, this.gameObject);
      }
    }
  }

  protected override void OnDestroy() => base.OnDestroy();
}
