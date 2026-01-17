// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUILayoutToggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.GUILayout)]
[HutongGames.PlayMaker.Tooltip("Makes an on/off Toggle Button and stores the button state in a Bool Variable.")]
public class GUILayoutToggle : GUILayoutAction
{
  [RequiredField]
  [UIHint(UIHint.Variable)]
  public FsmBool storeButtonState;
  public FsmTexture image;
  public FsmString text;
  public FsmString tooltip;
  public FsmString style;
  public FsmEvent changedEvent;

  public override void Reset()
  {
    base.Reset();
    this.storeButtonState = (FsmBool) null;
    this.text = (FsmString) string.Empty;
    this.image = (FsmTexture) null;
    this.tooltip = (FsmString) string.Empty;
    this.style = (FsmString) "Toggle";
    this.changedEvent = (FsmEvent) null;
  }

  public override void OnGUI()
  {
    bool changed = GUI.changed;
    GUI.changed = false;
    this.storeButtonState.Value = GUILayout.Toggle(this.storeButtonState.Value, new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), (GUIStyle) this.style.Value, this.LayoutOptions);
    if (GUI.changed)
    {
      this.Fsm.Event(this.changedEvent);
      GUIUtility.ExitGUI();
    }
    else
      GUI.changed = changed;
  }
}
