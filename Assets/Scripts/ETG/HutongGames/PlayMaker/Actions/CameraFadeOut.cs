// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.CameraFadeOut
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Camera)]
  [HutongGames.PlayMaker.Tooltip("Fade to a fullscreen Color. NOTE: Uses OnGUI so requires a PlayMakerGUI component in the scene.")]
  public class CameraFadeOut : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("Color to fade to. E.g., Fade to black.")]
    [RequiredField]
    public FsmColor color;
    [HutongGames.PlayMaker.Tooltip("Fade out time in seconds.")]
    [RequiredField]
    [HasFloatSlider(0.0f, 10f)]
    public FsmFloat time;
    [HutongGames.PlayMaker.Tooltip("Event to send when finished.")]
    public FsmEvent finishEvent;
    [HutongGames.PlayMaker.Tooltip("Ignore TimeScale. Useful if the game is paused.")]
    public bool realTime;
    private float startTime;
    private float currentTime;
    private Color colorLerp;

    public override void Reset()
    {
      this.color = (FsmColor) Color.black;
      this.time = (FsmFloat) 1f;
      this.finishEvent = (FsmEvent) null;
    }

    public override void OnEnter()
    {
      this.startTime = FsmTime.RealtimeSinceStartup;
      this.currentTime = 0.0f;
      this.colorLerp = Color.clear;
    }

    public override void OnUpdate()
    {
      if (this.realTime)
        this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
      else
        this.currentTime += Time.deltaTime;
      this.colorLerp = Color.Lerp(Color.clear, this.color.Value, this.currentTime / this.time.Value);
      if ((double) this.currentTime <= (double) this.time.Value || this.finishEvent == null)
        return;
      this.Fsm.Event(this.finishEvent);
    }

    public override void OnGUI()
    {
      Color color = GUI.color;
      GUI.color = this.colorLerp;
      GUI.DrawTexture(new Rect(0.0f, 0.0f, (float) Screen.width, (float) Screen.height), (Texture) ActionHelpers.WhiteTexture);
      GUI.color = color;
    }
  }
}
