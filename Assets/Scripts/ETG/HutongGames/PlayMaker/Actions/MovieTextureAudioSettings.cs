using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the Game Object as the Audio Source associated with the Movie Texture. The Game Object must have an AudioSource Component.")]
  [ActionCategory(ActionCategory.Movie)]
  public class MovieTextureAudioSettings : FsmStateAction
  {
    [RequiredField]
    [ObjectType(typeof (MovieTexture))]
    public FsmObject movieTexture;
    [RequiredField]
    [CheckForComponent(typeof (AudioSource))]
    public FsmGameObject gameObject;

    public override void Reset()
    {
      this.movieTexture = (FsmObject) null;
      this.gameObject = (FsmGameObject) null;
    }

    public override void OnEnter()
    {
      MovieTexture movieTexture = this.movieTexture.Value as MovieTexture;
      if ((Object) movieTexture != (Object) null && (Object) this.gameObject.Value != (Object) null)
      {
        AudioSource component = this.gameObject.Value.GetComponent<AudioSource>();
        if ((Object) component != (Object) null)
          component.clip = movieTexture.audioClip;
      }
      this.Finish();
    }
  }
}
