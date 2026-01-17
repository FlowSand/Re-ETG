// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUILayoutIntField
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("GUILayout Text Field to edit an Int Variable. Optionally send an event if the text has been edited.")]
[ActionCategory(ActionCategory.GUILayout)]
public class GUILayoutIntField : GUILayoutAction
{
  [HutongGames.PlayMaker.Tooltip("Int Variable to show in the edit field.")]
  [UIHint(UIHint.Variable)]
  public FsmInt intVariable;
  [HutongGames.PlayMaker.Tooltip("Optional GUIStyle in the active GUISKin.")]
  public FsmString style;
  [HutongGames.PlayMaker.Tooltip("Optional event to send when the value changes.")]
  public FsmEvent changedEvent;

  public override void Reset()
  {
    base.Reset();
    this.intVariable = (FsmInt) null;
    this.style = (FsmString) string.Empty;
    this.changedEvent = (FsmEvent) null;
  }

  public override void OnGUI()
  {
    bool changed = GUI.changed;
    GUI.changed = false;
    this.intVariable.Value = string.IsNullOrEmpty(this.style.Value) ? int.Parse(GUILayout.TextField(this.intVariable.Value.ToString(), this.LayoutOptions)) : int.Parse(GUILayout.TextField(this.intVariable.Value.ToString(), (GUIStyle) this.style.Value, this.LayoutOptions));
    if (GUI.changed)
    {
      this.Fsm.Event(this.changedEvent);
      GUIUtility.ExitGUI();
    }
    else
      GUI.changed = changed;
  }
}
