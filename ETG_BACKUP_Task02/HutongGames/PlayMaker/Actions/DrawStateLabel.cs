// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.DrawStateLabel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Debug)]
[Tooltip("Draws a state label for this FSM in the Game View. The label is drawn on the game object that owns the FSM. Use this to override the global setting in the PlayMaker Debug menu.")]
public class DrawStateLabel : FsmStateAction
{
  [RequiredField]
  [Tooltip("Set to True to show State labels, or Fals to hide them.")]
  public FsmBool showLabel;

  public override void Reset() => this.showLabel = (FsmBool) true;

  public override void OnEnter()
  {
    this.Fsm.ShowStateLabel = this.showLabel.Value;
    this.Finish();
  }
}
