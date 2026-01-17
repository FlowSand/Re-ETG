// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.TestFoyerShopStockDepleted
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".NPCs")]
  public class TestFoyerShopStockDepleted : FsmStateAction
  {
    public FsmEvent CurrentStockDepleted;
    public FsmEvent AllStockDepleted;
    public FsmEvent NotDepleted;
    private TalkDoerLite m_talkDoer;

    public override void Reset()
    {
      this.CurrentStockDepleted = (FsmEvent) null;
      this.AllStockDepleted = (FsmEvent) null;
      this.NotDepleted = (FsmEvent) null;
    }

    public override string ErrorCheck()
    {
      return FsmEvent.IsNullOrEmpty(this.CurrentStockDepleted) && FsmEvent.IsNullOrEmpty(this.AllStockDepleted) && FsmEvent.IsNullOrEmpty(this.NotDepleted) ? "Action sends no events!" : string.Empty;
    }

    public override void OnEnter()
    {
      this.m_talkDoer = this.Owner.GetComponent<TalkDoerLite>();
      this.DoCompare();
      this.Finish();
    }

    private void DoCompare()
    {
      if (this.m_talkDoer.ShopStockStatus == Tribool.Complete)
        this.Fsm.Event(this.AllStockDepleted);
      else if (this.m_talkDoer.ShopStockStatus == Tribool.Ready)
        this.Fsm.Event(this.CurrentStockDepleted);
      else
        this.Fsm.Event(this.NotDepleted);
    }
  }
}
