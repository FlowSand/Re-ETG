// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.KillDelayedEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Kill all queued delayed events. Normally delayed events are automatically killed when the active state is exited, but you can override this behaviour in FSM settings. If you choose to keep delayed events you can use this action to kill them when needed.")]
[ActionCategory(ActionCategory.StateMachine)]
[Note("Kill all queued delayed events.")]
public class KillDelayedEvents : FsmStateAction
{
  public override void OnEnter()
  {
    this.Fsm.KillDelayedEvents();
    this.Finish();
  }
}
