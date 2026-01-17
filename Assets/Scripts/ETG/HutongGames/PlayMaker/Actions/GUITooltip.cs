// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GUITooltip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the Tooltip of the control the mouse is currently over and store it in a String Variable.")]
  [ActionCategory(ActionCategory.GUI)]
  public class GUITooltip : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    public FsmString storeTooltip;

    public override void Reset() => this.storeTooltip = (FsmString) null;

    public override void OnGUI() => this.storeTooltip.Value = GUI.tooltip;
  }
}
