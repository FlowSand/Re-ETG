// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetGUIAlpha
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the global Alpha for the GUI. Useful for fading GUI up/down. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
[ActionCategory(ActionCategory.GUI)]
public class SetGUIAlpha : FsmStateAction
{
  [RequiredField]
  public FsmFloat alpha;
  public FsmBool applyGlobally;

  public override void Reset() => this.alpha = (FsmFloat) 1f;

  public override void OnGUI()
  {
    GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.alpha.Value);
    if (!this.applyGlobally.Value)
      return;
    PlayMakerGUI.GUIColor = GUI.color;
  }
}
