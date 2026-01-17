// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.PlayMovieTexture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
