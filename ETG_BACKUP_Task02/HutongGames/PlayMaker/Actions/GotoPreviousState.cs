// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GotoPreviousState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.StateMachine)]
[Tooltip("Immediately return to the previously active state.")]
public class GotoPreviousState : FsmStateAction
{
  public override void Reset()
  {
  }

  public override void OnEnter()
  {
    if (this.Fsm.PreviousActiveState != null)
    {
      this.Log("Goto Previous State: " + this.Fsm.PreviousActiveState.Name);
      this.Fsm.GotoPreviousState();
    }
    this.Finish();
  }
}
