// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.MunchCurrentGun
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Toss the current gun into the gunper monper and (hopefully) get an upgrade.")]
  [ActionCategory(".NPCs")]
  public class MunchCurrentGun : FsmStateAction
  {
    public FsmEvent rewardGivenEvent;
    public FsmEvent noRewardEvent;
    private GunberMuncherController m_muncher;

    public override void OnEnter()
    {
      TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
      this.m_muncher = component.GetComponent<GunberMuncherController>();
      this.m_muncher.TossPlayerEquippedGun(component.TalkingPlayer);
    }

    public override void OnUpdate()
    {
      if (this.m_muncher.IsProcessing)
        return;
      if (this.m_muncher.ShouldGiveReward)
        this.Fsm.Event(this.rewardGivenEvent);
      else
        this.Fsm.Event(this.noRewardEvent);
    }
  }
}
