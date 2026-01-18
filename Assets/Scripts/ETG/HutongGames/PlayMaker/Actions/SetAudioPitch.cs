using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the Pitch of the Audio Clip played by the AudioSource component on a Game Object.")]
  [ActionCategory(ActionCategory.Audio)]
  public class SetAudioPitch : ComponentAction<AudioSource>
  {
    [CheckForComponent(typeof (AudioSource))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    public FsmFloat pitch;
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.pitch = (FsmFloat) 1f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetAudioPitch();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetAudioPitch();

    private void DoSetAudioPitch()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)) || this.pitch.IsNone)
        return;
      this.audio.pitch = this.pitch.Value;
    }
  }
}
