using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Plays an Audio Clip at a position defined by a Game Object or Vector3. If a position is defined, it takes priority over the game object. This action doesn't require an Audio Source component, but offers less control than Audio actions.")]
  [ActionCategory(ActionCategory.Audio)]
  public class PlaySound : FsmStateAction
  {
    public FsmOwnerDefault gameObject;
    public FsmVector3 position;
    [Title("Audio Clip")]
    [RequiredField]
    [ObjectType(typeof (AudioClip))]
    public FsmObject clip;
    [HasFloatSlider(0.0f, 1f)]
    public FsmFloat volume = (FsmFloat) 1f;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      FsmVector3 fsmVector3 = new FsmVector3();
      fsmVector3.UseVariable = true;
      this.position = fsmVector3;
      this.clip = (FsmObject) null;
      this.volume = (FsmFloat) 1f;
    }

    public override void OnEnter()
    {
      this.DoPlaySound();
      this.Finish();
    }

    private void DoPlaySound()
    {
      AudioClip clip = this.clip.Value as AudioClip;
      if ((Object) clip == (Object) null)
        this.LogWarning("Missing Audio Clip!");
      else if (!this.position.IsNone)
      {
        AudioSource.PlayClipAtPoint(clip, this.position.Value, this.volume.Value);
      }
      else
      {
        GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
        if ((Object) ownerDefaultTarget == (Object) null)
          return;
        AudioSource.PlayClipAtPoint(clip, ownerDefaultTarget.transform.position, this.volume.Value);
      }
    }
  }
}
