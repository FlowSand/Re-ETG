// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUILayoutBeginVertical
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.GUILayout)]
  [HutongGames.PlayMaker.Tooltip("Begins a vertical control group. The group must be closed with GUILayoutEndVertical action.")]
  public class GUILayoutBeginVertical : GUILayoutAction
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
      GUILayout.BeginVertical(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), (GUIStyle) this.style.Value, this.LayoutOptions);
    }
  }
}
