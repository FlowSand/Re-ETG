// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TestTurboMode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(".Brave")]
[Tooltip("Sends Events based on whether or not the player is in turbo mode.")]
public class TestTurboMode : FsmStateAction
{
  [Tooltip("Event to send if turbo mode is active.")]
  public FsmEvent isTrue;
  [Tooltip("Event to send if turbo mode is inactive.")]
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
    this.Fsm.Event(!GameStatsManager.Instance.isTurboMode ? this.isFalse : this.isTrue);
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate()
  {
    this.Fsm.Event(!GameStatsManager.Instance.isTurboMode ? this.isFalse : this.isTrue);
  }
}
