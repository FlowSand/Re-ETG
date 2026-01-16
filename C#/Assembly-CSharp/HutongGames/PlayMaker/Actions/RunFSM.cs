// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.RunFSM
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.StateMachine)]
[HutongGames.PlayMaker.Tooltip("Creates an FSM from a saved FSM Template.")]
public class RunFSM : RunFSMAction
{
  public FsmTemplateControl fsmTemplateControl = new FsmTemplateControl();
  [UIHint(UIHint.Variable)]
  public FsmInt storeID;
  [HutongGames.PlayMaker.Tooltip("Event to send when the FSM has finished (usually because it ran a Finish FSM action).")]
  public FsmEvent finishEvent;

  public override void Reset()
  {
    this.fsmTemplateControl = new FsmTemplateControl();
    this.storeID = (FsmInt) null;
    this.runFsm = (Fsm) null;
  }

  public override void Awake()
  {
    if (!((Object) this.fsmTemplateControl.fsmTemplate != (Object) null) || !Application.isPlaying)
      return;
    this.runFsm = this.Fsm.CreateSubFsm(this.fsmTemplateControl);
  }

  public override void OnEnter()
  {
    if (this.runFsm == null)
    {
      this.Finish();
    }
    else
    {
      this.fsmTemplateControl.UpdateValues();
      this.fsmTemplateControl.ApplyOverrides(this.runFsm);
      this.runFsm.OnEnable();
      if (!this.runFsm.Started)
        this.runFsm.Start();
      this.storeID.Value = this.fsmTemplateControl.ID;
      this.CheckIfFinished();
    }
  }

  protected override void CheckIfFinished()
  {
    if (this.runFsm != null && !this.runFsm.Finished)
      return;
    this.Finish();
    this.Fsm.Event(this.finishEvent);
  }
}
