// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetLastEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Gets the event that caused the transition to the current state, and stores it in a String Variable.")]
[ActionCategory(ActionCategory.StateMachine)]
public class GetLastEvent : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  public FsmString storeEvent;

  public override void Reset() => this.storeEvent = (FsmString) null;

  public override void OnEnter()
  {
    this.storeEvent.Value = this.Fsm.LastTransition != null ? this.Fsm.LastTransition.EventName : "START";
    this.Finish();
  }
}
