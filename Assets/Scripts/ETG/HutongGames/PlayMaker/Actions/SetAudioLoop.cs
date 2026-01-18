using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Audio)]
  [HutongGames.PlayMaker.Tooltip("Sets looping on the AudioSource component on a Game Object.")]
  public class SetAudioLoop : ComponentAction<AudioSource>
  {
    [CheckForComponent(typeof (AudioSource))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    public FsmBool loop;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.loop = (FsmBool) false;
    }

    public override void OnEnter()
    {
      if (this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        this.audio.loop = this.loop.Value;
      this.Finish();
    }
  }
}
