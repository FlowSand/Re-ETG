// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.DrawFullscreenColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Fills the screen with a Color. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
[ActionCategory(ActionCategory.GUI)]
public class DrawFullscreenColor : FsmStateAction
{
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("Color. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
  public FsmColor color;

  public override void Reset() => this.color = (FsmColor) Color.white;

  public override void OnGUI()
  {
    Color color = GUI.color;
    GUI.color = this.color.Value;
    GUI.DrawTexture(new Rect(0.0f, 0.0f, (float) Screen.width, (float) Screen.height), (Texture) ActionHelpers.WhiteTexture);
    GUI.color = color;
  }
}
