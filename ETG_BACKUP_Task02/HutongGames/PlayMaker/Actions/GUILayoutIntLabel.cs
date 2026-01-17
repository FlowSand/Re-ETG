// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUILayoutIntLabel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("GUILayout Label for an Int Variable.")]
[ActionCategory(ActionCategory.GUILayout)]
public class GUILayoutIntLabel : GUILayoutAction
{
  [HutongGames.PlayMaker.Tooltip("Text to put before the int variable.")]
  public FsmString prefix;
  [HutongGames.PlayMaker.Tooltip("Int variable to display.")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmInt intVariable;
  [HutongGames.PlayMaker.Tooltip("Optional GUIStyle in the active GUISKin.")]
  public FsmString style;

  public override void Reset()
  {
    base.Reset();
    this.prefix = (FsmString) string.Empty;
    this.style = (FsmString) string.Empty;
    this.intVariable = (FsmInt) null;
  }

  public override void OnGUI()
  {
    if (string.IsNullOrEmpty(this.style.Value))
      GUILayout.Label(new GUIContent(this.prefix.Value + (object) this.intVariable.Value), this.LayoutOptions);
    else
      GUILayout.Label(new GUIContent(this.prefix.Value + (object) this.intVariable.Value), (GUIStyle) this.style.Value, this.LayoutOptions);
  }
}
