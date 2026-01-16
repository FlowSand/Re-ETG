// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUILayoutBeginHorizontal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("GUILayout BeginHorizontal.")]
[ActionCategory(ActionCategory.GUILayout)]
public class GUILayoutBeginHorizontal : GUILayoutAction
{
  public FsmTexture image;
  public FsmString text;
  public FsmString tooltip;
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
    GUILayout.BeginHorizontal(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), (GUIStyle) this.style.Value, this.LayoutOptions);
  }
}
