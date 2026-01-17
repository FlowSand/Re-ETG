// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.RestartWhenFinished
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("When all other actions on this state are finished, send a RESTART event.")]
  [ActionCategory(".Brave")]
  public class RestartWhenFinished : FsmStateAction, INonFinishingState
  {
    public override string ErrorCheck()
    {
      string empty = string.Empty;
      this.Fsm.GetEvent("RESTART");
      return empty + BravePlayMakerUtility.CheckGlobalTransitionExists(this.Fsm, "RESTART");
    }

    public override void OnEnter()
    {
      if (!BravePlayMakerUtility.AllOthersAreFinished((FsmStateAction) this))
        return;
      this.GoToStartState();
    }

    public override void OnUpdate()
    {
      if (!BravePlayMakerUtility.AllOthersAreFinished((FsmStateAction) this))
        return;
      this.GoToStartState();
    }

    private void GoToStartState()
    {
      if (this.Fsm.SuppressGlobalTransitions)
      {
        foreach (FsmStateAction action in this.State.Actions)
        {
          if (action is ResumeGlobalTransitions)
          {
            this.Fsm.SuppressGlobalTransitions = false;
            break;
          }
        }
      }
      this.Fsm.Event("RESTART");
      this.Finish();
    }
  }
}
