// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUILayoutTextField
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GUILayout)]
[HutongGames.PlayMaker.Tooltip("GUILayout Text Field. Optionally send an event if the text has been edited.")]
public class GUILayoutTextField : GUILayoutAction
{
  [UIHint(UIHint.Variable)]
  public FsmString text;
  public FsmInt maxLength;
  public FsmString style;
  public FsmEvent changedEvent;

  public override void Reset()
  {
    base.Reset();
    this.text = (FsmString) null;
    this.maxLength = (FsmInt) 25;
    this.style = (FsmString) "TextField";
    this.changedEvent = (FsmEvent) null;
  }

  public override void OnGUI()
  {
    bool changed = GUI.changed;
    GUI.changed = false;
    this.text.Value = GUILayout.TextField(this.text.Value, this.maxLength.Value, (GUIStyle) this.style.Value, this.LayoutOptions);
    if (GUI.changed)
    {
      this.Fsm.Event(this.changedEvent);
      GUIUtility.ExitGUI();
    }
    else
      GUI.changed = changed;
  }
}
