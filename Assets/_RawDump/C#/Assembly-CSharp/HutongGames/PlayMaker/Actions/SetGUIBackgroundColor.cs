// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetGUIBackgroundColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the Tinting Color for all background elements rendered by the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
[ActionCategory(ActionCategory.GUI)]
public class SetGUIBackgroundColor : FsmStateAction
{
  [RequiredField]
  public FsmColor backgroundColor;
  public FsmBool applyGlobally;

  public override void Reset() => this.backgroundColor = (FsmColor) Color.white;

  public override void OnGUI()
  {
    GUI.backgroundColor = this.backgroundColor.Value;
    if (!this.applyGlobally.Value)
      return;
    PlayMakerGUI.GUIBackgroundColor = GUI.backgroundColor;
  }
}
