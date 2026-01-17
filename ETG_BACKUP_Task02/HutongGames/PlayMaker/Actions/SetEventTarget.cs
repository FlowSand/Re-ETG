// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SetEventTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Sets the target FSM for all subsequent events sent by this state. The default 'Self' sends events to this FSM.")]
[ActionCategory(ActionCategory.StateMachine)]
public class SetEventTarget : FsmStateAction
{
  public FsmEventTarget eventTarget;

  public override void Reset()
  {
  }

  public override void OnEnter()
  {
    this.Fsm.EventTarget = this.eventTarget;
    this.Finish();
  }
}
