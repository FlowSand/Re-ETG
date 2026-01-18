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
