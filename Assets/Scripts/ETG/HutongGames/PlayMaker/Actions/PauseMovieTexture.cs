// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.PauseMovieTexture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
