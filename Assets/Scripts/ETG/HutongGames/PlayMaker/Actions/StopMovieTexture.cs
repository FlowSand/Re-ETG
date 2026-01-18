using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Movie)]
    [HutongGames.PlayMaker.Tooltip("Stops playing the Movie Texture, and rewinds it to the beginning.")]
    public class StopMovieTexture : FsmStateAction
    {
        [ObjectType(typeof (MovieTexture))]
        [RequiredField]
        public FsmObject movieTexture;

        public override void Reset() => this.movieTexture = (FsmObject) null;

        public override void OnEnter()
        {
            MovieTexture movieTexture = this.movieTexture.Value as MovieTexture;
            if ((Object) movieTexture != (Object) null)
                movieTexture.Stop();
            this.Finish();
        }
    }
}
