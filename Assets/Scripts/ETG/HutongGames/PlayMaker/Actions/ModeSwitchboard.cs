// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ModeSwitchboard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Switchboard to jump to different NPC modes.")]
  [ActionCategory(".Brave")]
  public class ModeSwitchboard : FsmStateAction
  {
    public override string ErrorCheck()
    {
      string str = string.Empty + BravePlayMakerUtility.CheckCurrentModeVariable(this.Fsm);
      FsmString fsmString = this.Fsm.Variables.GetFsmString("currentMode");
      return str + BravePlayMakerUtility.CheckEventExists(this.Fsm, fsmString.Value) + BravePlayMakerUtility.CheckGlobalTransitionExists(this.Fsm, fsmString.Value);
    }

    public override void OnEnter()
    {
      this.Fsm.Event(this.Fsm.Variables.GetFsmString("currentMode").Value);
      this.Finish();
    }
  }
}
