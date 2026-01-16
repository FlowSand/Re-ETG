// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUILayoutBox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("GUILayout Box.")]
[ActionCategory(ActionCategory.GUILayout)]
public class GUILayoutBox : GUILayoutAction
{
  [HutongGames.PlayMaker.Tooltip("Image to display in the Box.")]
  public FsmTexture image;
  [HutongGames.PlayMaker.Tooltip("Text to display in the Box.")]
  public FsmString text;
  [HutongGames.PlayMaker.Tooltip("Optional Tooltip string.")]
  public FsmString tooltip;
  [HutongGames.PlayMaker.Tooltip("Optional GUIStyle in the active GUISkin.")]
  public FsmString style;

  public override void Reset()
  {
    base.Reset();
    this.text = (FsmString) string.Empty;
    this.image = (FsmTexture) null;
    this.tooltip = (FsmString) string.Empty;
    this.style = (FsmString) string.Empty;
  }

  public override void OnGUI()
  {
    if (string.IsNullOrEmpty(this.style.Value))
      GUILayout.Box(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.LayoutOptions);
    else
      GUILayout.Box(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), (GUIStyle) this.style.Value, this.LayoutOptions);
  }
}
