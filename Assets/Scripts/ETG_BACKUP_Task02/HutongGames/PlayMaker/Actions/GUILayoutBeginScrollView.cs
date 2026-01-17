// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUILayoutBeginScrollView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GUILayout)]
[HutongGames.PlayMaker.Tooltip("Begins a ScrollView. Use GUILayoutEndScrollView at the end of the block.")]
public class GUILayoutBeginScrollView : GUILayoutAction
{
  [UIHint(UIHint.Variable)]
  [RequiredField]
  [HutongGames.PlayMaker.Tooltip("Assign a Vector2 variable to store the scroll position of this view.")]
  public FsmVector2 scrollPosition;
  [HutongGames.PlayMaker.Tooltip("Always show the horizontal scrollbars.")]
  public FsmBool horizontalScrollbar;
  [HutongGames.PlayMaker.Tooltip("Always show the vertical scrollbars.")]
  public FsmBool verticalScrollbar;
  [HutongGames.PlayMaker.Tooltip("Define custom styles below. NOTE: You have to define all the styles if you check this option.")]
  public FsmBool useCustomStyle;
  [HutongGames.PlayMaker.Tooltip("Named style in the active GUISkin for the horizontal scrollbars.")]
  public FsmString horizontalStyle;
  [HutongGames.PlayMaker.Tooltip("Named style in the active GUISkin for the vertical scrollbars.")]
  public FsmString verticalStyle;
  [HutongGames.PlayMaker.Tooltip("Named style in the active GUISkin for the background.")]
  public FsmString backgroundStyle;

  public override void Reset()
  {
    base.Reset();
    this.scrollPosition = (FsmVector2) null;
    this.horizontalScrollbar = (FsmBool) null;
    this.verticalScrollbar = (FsmBool) null;
    this.useCustomStyle = (FsmBool) null;
    this.horizontalStyle = (FsmString) null;
    this.verticalStyle = (FsmString) null;
    this.backgroundStyle = (FsmString) null;
  }

  public override void OnGUI()
  {
    if (this.useCustomStyle.Value)
      this.scrollPosition.Value = GUILayout.BeginScrollView(this.scrollPosition.Value, this.horizontalScrollbar.Value, this.verticalScrollbar.Value, (GUIStyle) this.horizontalStyle.Value, (GUIStyle) this.verticalStyle.Value, (GUIStyle) this.backgroundStyle.Value, this.LayoutOptions);
    else
      this.scrollPosition.Value = GUILayout.BeginScrollView(this.scrollPosition.Value, this.horizontalScrollbar.Value, this.verticalScrollbar.Value, this.LayoutOptions);
  }
}
