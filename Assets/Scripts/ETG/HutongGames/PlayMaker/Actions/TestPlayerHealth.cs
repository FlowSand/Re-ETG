// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TestPlayerHealth
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(".NPCs")]
[Tooltip("Checks whether or not the player has a certain amount of health.")]
public class TestPlayerHealth : FsmStateAction
{
  [Tooltip("Check Percent")]
  public FsmBool UsePercentage;
  [Tooltip("Value to check.")]
  public FsmFloat value;
  [Tooltip("Event sent if the amount is greater than <value>.")]
  public FsmEvent greaterThan;
  [Tooltip("Event sent if the amount is greater than or equal to <value>.")]
  public FsmEvent greaterThanOrEqual;
  [Tooltip("Event sent if the amount equals <value>.")]
  public FsmEvent equal;
  [Tooltip("Event sent if the amount is less than or equal to <value>.")]
  public FsmEvent lessThanOrEqual;
  [Tooltip("Event sent if the amount is less than <value>.")]
  public FsmEvent lessThan;
  public bool everyFrame;
  private TalkDoerLite m_talkDoer;

  public override void Reset()
  {
    this.UsePercentage = (FsmBool) false;
    this.value = (FsmFloat) 0.0f;
    this.greaterThan = (FsmEvent) null;
    this.greaterThanOrEqual = (FsmEvent) null;
    this.equal = (FsmEvent) null;
    this.lessThanOrEqual = (FsmEvent) null;
    this.lessThan = (FsmEvent) null;
    this.everyFrame = false;
  }

  public override string ErrorCheck()
  {
    return FsmEvent.IsNullOrEmpty(this.greaterThan) && FsmEvent.IsNullOrEmpty(this.greaterThanOrEqual) && FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.lessThanOrEqual) && FsmEvent.IsNullOrEmpty(this.lessThan) ? "Action sends no events!" : string.Empty;
  }

  public override void OnEnter()
  {
    this.m_talkDoer = this.Owner.GetComponent<TalkDoerLite>();
    this.DoCompare();
    if (this.everyFrame)
      return;
    this.Finish();
  }

  public override void OnUpdate() => this.DoCompare();

  private void DoCompare()
  {
    float num = this.m_talkDoer.TalkingPlayer.healthHaver.GetCurrentHealth();
    if (this.UsePercentage.Value)
      num = this.m_talkDoer.TalkingPlayer.healthHaver.GetCurrentHealthPercentage();
    if ((double) num > (double) this.value.Value)
      this.Fsm.Event(this.greaterThan);
    if ((double) num >= (double) this.value.Value)
      this.Fsm.Event(this.greaterThanOrEqual);
    if ((double) num == (double) this.value.Value)
      this.Fsm.Event(this.equal);
    if ((double) num <= (double) this.value.Value)
      this.Fsm.Event(this.lessThanOrEqual);
    if ((double) num >= (double) this.value.Value)
      return;
    this.Fsm.Event(this.lessThan);
  }
}
