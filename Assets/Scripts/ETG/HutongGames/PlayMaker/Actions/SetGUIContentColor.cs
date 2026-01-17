// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetGUIContentColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the Tinting Color for all text rendered by the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
[ActionCategory(ActionCategory.GUI)]
public class SetGUIContentColor : FsmStateAction
{
  [RequiredField]
  public FsmColor contentColor;
  public FsmBool applyGlobally;

  public override void Reset() => this.contentColor = (FsmColor) Color.white;

  public override void OnGUI()
  {
    GUI.contentColor = this.contentColor.Value;
    if (!this.applyGlobally.Value)
      return;
    PlayMakerGUI.GUIContentColor = GUI.contentColor;
  }
}
