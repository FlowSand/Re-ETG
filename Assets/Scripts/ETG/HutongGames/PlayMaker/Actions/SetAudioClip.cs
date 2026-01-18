using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the Audio Clip played by the AudioSource component on a Game Object.")]
  [ActionCategory(ActionCategory.Audio)]
  public class SetAudioClip : ComponentAction<AudioSource>
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject with the AudioSource component.")]
    [CheckForComponent(typeof (AudioSource))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The AudioClip to set.")]
    [ObjectType(typeof (AudioClip))]
    public FsmObject audioClip;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.audioClip = (FsmObject) null;
    }

    public override void OnEnter()
    {
      if (this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        this.audio.clip = this.audioClip.Value as AudioClip;
      this.Finish();
    }
  }
}
