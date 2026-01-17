// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUILayoutHorizontalSlider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("A Horizontal Slider linked to a Float Variable.")]
[ActionCategory(ActionCategory.GUILayout)]
public class GUILayoutHorizontalSlider : GUILayoutAction
{
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmFloat floatVariable;
  [RequiredField]
  public FsmFloat leftValue;
  [RequiredField]
  public FsmFloat rightValue;
  public FsmEvent changedEvent;

  public override void Reset()
  {
    base.Reset();
    this.floatVariable = (FsmFloat) null;
    this.leftValue = (FsmFloat) 0.0f;
    this.rightValue = (FsmFloat) 100f;
    this.changedEvent = (FsmEvent) null;
  }

  public override void OnGUI()
  {
    bool changed = GUI.changed;
    GUI.changed = false;
    if (this.floatVariable != null)
      this.floatVariable.Value = GUILayout.HorizontalSlider(this.floatVariable.Value, this.leftValue.Value, this.rightValue.Value, this.LayoutOptions);
    if (GUI.changed)
    {
      this.Fsm.Event(this.changedEvent);
      GUIUtility.ExitGUI();
    }
    else
      GUI.changed = changed;
  }
}
