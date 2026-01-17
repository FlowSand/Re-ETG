// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.StopMovieTexture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
