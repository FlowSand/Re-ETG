// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ResumeGlobalTransitions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".Brave")]
  [Tooltip("Allows the FSM to fire global transitions again.")]
  public class ResumeGlobalTransitions : FsmStateAction, INonFinishingState
  {
    public override void OnEnter()
    {
      if (!BravePlayMakerUtility.AllOthersAreFinished((FsmStateAction) this))
        return;
      this.Fsm.SuppressGlobalTransitions = false;
      this.Finish();
    }

    public override void OnUpdate()
    {
      if (!BravePlayMakerUtility.AllOthersAreFinished((FsmStateAction) this))
        return;
      this.Fsm.SuppressGlobalTransitions = false;
      this.Finish();
    }
  }
}
