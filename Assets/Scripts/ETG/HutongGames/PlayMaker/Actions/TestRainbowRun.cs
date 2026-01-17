// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TestRainbowRun
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Sends Events based on whether or not the player is in rainbow mode.")]
  [ActionCategory(".Brave")]
  public class TestRainbowRun : FsmStateAction
  {
    [Tooltip("Event to send if rainbow mode is active.")]
    public FsmEvent isTrue;
    [Tooltip("Event to send if rainbow mode is inactive.")]
    public FsmEvent isFalse;
    [Tooltip("Repeat every frame while the state is active.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.isTrue = (FsmEvent) null;
      this.isFalse = (FsmEvent) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.Fsm.Event(!GameStatsManager.Instance.rainbowRunToggled ? this.isFalse : this.isTrue);
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      this.Fsm.Event(!GameStatsManager.Instance.rainbowRunToggled ? this.isFalse : this.isTrue);
    }
  }
}
