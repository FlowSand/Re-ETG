using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Pauses a Movie Texture.")]
  [ActionCategory(ActionCategory.Movie)]
  public class PauseMovieTexture : FsmStateAction
  {
    [ObjectType(typeof (MovieTexture))]
    [RequiredField]
    public FsmObject movieTexture;

    public override void Reset() => this.movieTexture = (FsmObject) null;

    public override void OnEnter()
    {
      MovieTexture movieTexture = this.movieTexture.Value as MovieTexture;
      if ((Object) movieTexture != (Object) null)
        movieTexture.Pause();
      this.Finish();
    }
  }
}
