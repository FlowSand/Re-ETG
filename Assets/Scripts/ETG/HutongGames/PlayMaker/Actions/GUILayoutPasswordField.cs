// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUILayoutPasswordField
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
[ActionCategory(ActionCategory.GUILayout)]
public class GUILayoutPasswordField : GUILayoutAction
{
  [HutongGames.PlayMaker.Tooltip("The password Text")]
  [UIHint(UIHint.Variable)]
  public FsmString text;
  [HutongGames.PlayMaker.Tooltip("The Maximum Length of the field")]
  public FsmInt maxLength;
  [HutongGames.PlayMaker.Tooltip("The Style of the Field")]
  public FsmString style;
  [HutongGames.PlayMaker.Tooltip("Event sent when field content changed")]
  public FsmEvent changedEvent;
  [HutongGames.PlayMaker.Tooltip("Replacement character to hide the password")]
  public FsmString mask;

  public override void Reset()
  {
    this.text = (FsmString) null;
    this.maxLength = (FsmInt) 25;
    this.style = (FsmString) "TextField";
    this.mask = (FsmString) "*";
    this.changedEvent = (FsmEvent) null;
  }

  public override void OnGUI()
  {
    bool changed = GUI.changed;
    GUI.changed = false;
    this.text.Value = GUILayout.PasswordField(this.text.Value, this.mask.Value[0], (GUIStyle) this.style.Value, this.LayoutOptions);
    if (GUI.changed)
    {
      this.Fsm.Event(this.changedEvent);
      GUIUtility.ExitGUI();
    }
    else
      GUI.changed = changed;
  }
}
