// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.NextFrameEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Sends an Event in the next frame. Useful if you want to loop states every frame.")]
[ActionCategory(ActionCategory.StateMachine)]
public class NextFrameEvent : FsmStateAction
{
  [RequiredField]
  public FsmEvent sendEvent;

  public override void Reset() => this.sendEvent = (FsmEvent) null;

  public override void OnEnter()
  {
  }

  public override void OnUpdate()
  {
    this.Finish();
    this.Fsm.Event(this.sendEvent);
  }
}
