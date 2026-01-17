// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.AudioPlay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionTarget(typeof (AudioSource), "gameObject", false)]
  [ActionTarget(typeof (AudioClip), "oneShotClip", false)]
  [HutongGames.PlayMaker.Tooltip("Plays the Audio Clip set with Set Audio Clip or in the Audio Source inspector on a Game Object. Optionally plays a one shot Audio Clip.")]
  [ActionCategory(ActionCategory.Audio)]
  public class AudioPlay : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject with an AudioSource component.")]
    [RequiredField]
    [CheckForComponent(typeof (AudioSource))]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("Set the volume.")]
    [HasFloatSlider(0.0f, 1f)]
    public FsmFloat volume;
    [HutongGames.PlayMaker.Tooltip("Optionally play a 'one shot' AudioClip. NOTE: Volume cannot be adjusted while playing a 'one shot' AudioClip.")]
    [ObjectType(typeof (AudioClip))]
    public FsmObject oneShotClip;
    [HutongGames.PlayMaker.Tooltip("Event to send when the AudioClip finishes playing.")]
    public FsmEvent finishedEvent;
    private AudioSource audio;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.volume = (FsmFloat) 1f;
      this.oneShotClip = (FsmObject) null;
      this.finishedEvent = (FsmEvent) null;
    }

    public override void OnEnter()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget != (Object) null)
      {
        this.audio = ownerDefaultTarget.GetComponent<AudioSource>();
        if ((Object) this.audio != (Object) null)
        {
          AudioClip clip = this.oneShotClip.Value as AudioClip;
          if ((Object) clip == (Object) null)
          {
            this.audio.Play();
            if (this.volume.IsNone)
              return;
            this.audio.volume = this.volume.Value;
            return;
          }
          if (!this.volume.IsNone)
          {
            this.audio.PlayOneShot(clip, this.volume.Value);
            return;
          }
          this.audio.PlayOneShot(clip);
          return;
        }
      }
      this.Finish();
    }

    public override void OnUpdate()
    {
      if ((Object) this.audio == (Object) null)
        this.Finish();
      else if (!this.audio.isPlaying)
      {
        this.Fsm.Event(this.finishedEvent);
        this.Finish();
      }
      else
      {
        if (this.volume.IsNone || (double) this.volume.Value == (double) this.audio.volume)
          return;
        this.audio.volume = this.volume.Value;
      }
    }
  }
}
