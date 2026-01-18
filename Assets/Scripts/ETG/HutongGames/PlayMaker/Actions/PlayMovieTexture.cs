using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Plays a Movie Texture. Use the Movie Texture in a Material, or in the GUI.")]
  [ActionCategory(ActionCategory.Movie)]
  public class PlayMovieTexture : FsmStateAction
  {
    [RequiredField]
    [ObjectType(typeof (MovieTexture))]
    public FsmObject movieTexture;
    public FsmBool loop;

    public override void Reset()
    {
      this.movieTexture = (FsmObject) null;
      this.loop = (FsmBool) false;
    }

    public override void OnEnter()
    {
      MovieTexture movieTexture = this.movieTexture.Value as MovieTexture;
      if ((Object) movieTexture != (Object) null)
      {
        movieTexture.loop = this.loop.Value;
        movieTexture.Play();
      }
      this.Finish();
    }
  }
}
