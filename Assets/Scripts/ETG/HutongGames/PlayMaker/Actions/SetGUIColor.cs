// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetGUIColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.GUI)]
  [HutongGames.PlayMaker.Tooltip("Sets the Tinting Color for the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
  public class SetGUIColor : FsmStateAction
  {
    [RequiredField]
    public FsmColor color;
    public FsmBool applyGlobally;

    public override void Reset() => this.color = (FsmColor) Color.white;

    public override void OnGUI()
    {
      GUI.color = this.color.Value;
      if (!this.applyGlobally.Value)
        return;
      PlayMakerGUI.GUIColor = GUI.color;
    }
  }
}
