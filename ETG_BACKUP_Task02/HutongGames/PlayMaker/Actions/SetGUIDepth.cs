// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetGUIDepth
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[HutongGames.PlayMaker.Tooltip("Sets the sorting depth of subsequent GUI elements.")]
[ActionCategory(ActionCategory.GUI)]
public class SetGUIDepth : FsmStateAction
{
  [RequiredField]
  public FsmInt depth;

  public override void Reset() => this.depth = (FsmInt) 0;

  public override void OnPreprocess() => this.Fsm.HandleOnGUI = true;

  public override void OnGUI() => GUI.depth = this.depth.Value;
}
