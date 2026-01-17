// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.FinishFSM
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Stop this FSM. If this FSM was launched by a Run FSM action, it will trigger a Finish event in that state.")]
  [Note("Stop this FSM. If this FSM was launched by a Run FSM action, it will trigger a Finish event in that state.")]
  [ActionCategory(ActionCategory.StateMachine)]
  public class FinishFSM : FsmStateAction
  {
    public override void OnEnter() => this.Fsm.Stop();
  }
}
