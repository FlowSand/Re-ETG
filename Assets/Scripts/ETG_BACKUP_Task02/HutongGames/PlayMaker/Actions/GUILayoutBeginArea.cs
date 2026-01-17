// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUILayoutBeginArea
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Begin a GUILayout block of GUI controls in a fixed screen area. NOTE: Block must end with a corresponding GUILayoutEndArea.")]
[ActionCategory(ActionCategory.GUILayout)]
public class GUILayoutBeginArea : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  public FsmRect screenRect;
  public FsmFloat left;
  public FsmFloat top;
  public FsmFloat width;
  public FsmFloat height;
  public FsmBool normalized;
  public FsmString style;
  private Rect rect;

  public override void Reset()
  {
    this.screenRect = (FsmRect) null;
    this.left = (FsmFloat) 0.0f;
    this.top = (FsmFloat) 0.0f;
    this.width = (FsmFloat) 1f;
    this.height = (FsmFloat) 1f;
    this.normalized = (FsmBool) true;
    this.style = (FsmString) string.Empty;
  }

  public override void OnGUI()
  {
    this.rect = this.screenRect.IsNone ? new Rect() : this.screenRect.Value;
    if (!this.left.IsNone)
      this.rect.x = this.left.Value;
    if (!this.top.IsNone)
      this.rect.y = this.top.Value;
    if (!this.width.IsNone)
      this.rect.width = this.width.Value;
    if (!this.height.IsNone)
      this.rect.height = this.height.Value;
    if (this.normalized.Value)
    {
      this.rect.x *= (float) Screen.width;
      this.rect.width *= (float) Screen.width;
      this.rect.y *= (float) Screen.height;
      this.rect.height *= (float) Screen.height;
    }
    GUILayout.BeginArea(this.rect, GUIContent.none, (GUIStyle) this.style.Value);
  }
}
