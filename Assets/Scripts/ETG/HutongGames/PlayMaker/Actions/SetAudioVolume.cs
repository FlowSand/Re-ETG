using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the Volume of the Audio Clip played by the AudioSource component on a Game Object.")]
  [ActionCategory(ActionCategory.Audio)]
  public class SetAudioVolume : ComponentAction<AudioSource>
  {
    [CheckForComponent(typeof (AudioSource))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HasFloatSlider(0.0f, 1f)]
    public FsmFloat volume;
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.volume = (FsmFloat) 1f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetAudioVolume();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetAudioVolume();

    private void DoSetAudioVolume()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)) || this.volume.IsNone)
        return;
      this.audio.volume = this.volume.Value;
    }
  }
}
