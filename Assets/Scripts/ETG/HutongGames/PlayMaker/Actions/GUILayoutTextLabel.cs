// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUILayoutTextLabel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("GUILayout Label for simple text.")]
  [ActionCategory(ActionCategory.GUILayout)]
  public class GUILayoutTextLabel : GUILayoutAction
  {
    [HutongGames.PlayMaker.Tooltip("Text to display.")]
    public FsmString text;
    [HutongGames.PlayMaker.Tooltip("Optional GUIStyle in the active GUISkin.")]
    public FsmString style;

    public override void Reset()
    {
      base.Reset();
      this.text = (FsmString) string.Empty;
      this.style = (FsmString) string.Empty;
    }

    public override void OnGUI()
    {
      if (string.IsNullOrEmpty(this.style.Value))
        GUILayout.Label(new GUIContent(this.text.Value), this.LayoutOptions);
      else
        GUILayout.Label(new GUIContent(this.text.Value), (GUIStyle) this.style.Value, this.LayoutOptions);
    }
  }
}
