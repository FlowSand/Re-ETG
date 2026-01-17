// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.WWWObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets data from a url and store it in variables. See Unity WWW docs for more details.")]
  [ActionCategory("Web Player")]
  public class WWWObject : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Url to download data from.")]
    [RequiredField]
    public FsmString url;
    [ActionSection("Results")]
    [UIHint(UIHint.Variable)]
    [HutongGames.PlayMaker.Tooltip("Gets text from the url.")]
    public FsmString storeText;
    [HutongGames.PlayMaker.Tooltip("Gets a Texture from the url.")]
    [UIHint(UIHint.Variable)]
    public FsmTexture storeTexture;
    [HutongGames.PlayMaker.Tooltip("Gets a Texture from the url.")]
    [ObjectType(typeof (MovieTexture))]
    [UIHint(UIHint.Variable)]
    public FsmObject storeMovieTexture;
    [HutongGames.PlayMaker.Tooltip("Error message if there was an error during the download.")]
    [UIHint(UIHint.Variable)]
    public FsmString errorString;
    [HutongGames.PlayMaker.Tooltip("How far the download progressed (0-1).")]
    [UIHint(UIHint.Variable)]
    public FsmFloat progress;
    [ActionSection("Events")]
    [HutongGames.PlayMaker.Tooltip("Event to send when the data has finished loading (progress = 1).")]
    public FsmEvent isDone;
    [HutongGames.PlayMaker.Tooltip("Event to send if there was an error.")]
    public FsmEvent isError;
    private WWW wwwObject;

    public override void Reset()
    {
      this.url = (FsmString) null;
      this.storeText = (FsmString) null;
      this.storeTexture = (FsmTexture) null;
      this.errorString = (FsmString) null;
      this.progress = (FsmFloat) null;
      this.isDone = (FsmEvent) null;
    }

    public override void OnEnter()
    {
      if (string.IsNullOrEmpty(this.url.Value))
        this.Finish();
      else
        this.wwwObject = new WWW(this.url.Value);
    }

    public override void OnUpdate()
    {
      if (this.wwwObject == null)
      {
        this.errorString.Value = "WWW Object is Null!";
        this.Finish();
      }
      else
      {
        this.errorString.Value = this.wwwObject.error;
        if (!string.IsNullOrEmpty(this.wwwObject.error))
        {
          this.Finish();
          this.Fsm.Event(this.isError);
        }
        else
        {
          this.progress.Value = this.wwwObject.progress;
          if (!this.progress.Value.Equals(1f))
            return;
          this.storeText.Value = this.wwwObject.text;
          this.storeTexture.Value = (Texture) this.wwwObject.texture;
          this.storeMovieTexture.Value = (Object) this.wwwObject.GetMovieTexture();
          this.errorString.Value = this.wwwObject.error;
          this.Fsm.Event(!string.IsNullOrEmpty(this.errorString.Value) ? this.isError : this.isDone);
          this.Finish();
        }
      }
    }
  }
}
