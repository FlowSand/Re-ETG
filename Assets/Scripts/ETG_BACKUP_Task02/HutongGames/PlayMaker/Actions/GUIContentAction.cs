// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUIContentAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("GUI base action - don't use!")]
public abstract class GUIContentAction : GUIAction
{
  public FsmTexture image;
  public FsmString text;
  public FsmString tooltip;
  public FsmString style;
  internal GUIContent content;

  public override void Reset()
  {
    base.Reset();
    this.image = (FsmTexture) null;
    this.text = (FsmString) string.Empty;
    this.tooltip = (FsmString) string.Empty;
    this.style = (FsmString) string.Empty;
  }

  public override void OnGUI()
  {
    base.OnGUI();
    this.content = new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value);
  }
}
