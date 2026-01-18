#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Checks whether or not the player has a certain amount of money.")]
  [ActionCategory(".NPCs")]
  public class MonsterHuntSwitch : FsmStateAction
  {
    public FsmEvent NeedsNewHunt;
    public FsmEvent HuntIncomplete;
    public FsmEvent HuntComplete;
    public bool everyFrame;

    public override void Reset()
    {
      this.NeedsNewHunt = (FsmEvent) null;
      this.HuntIncomplete = (FsmEvent) null;
      this.HuntComplete = (FsmEvent) null;
      this.everyFrame = false;
    }

    public override string ErrorCheck()
    {
      return FsmEvent.IsNullOrEmpty(this.NeedsNewHunt) && FsmEvent.IsNullOrEmpty(this.HuntIncomplete) && FsmEvent.IsNullOrEmpty(this.HuntComplete) ? "Action sends no events!" : string.Empty;
    }

    public override void OnEnter()
    {
      this.DoCompare();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoCompare();

    private void DoCompare()
    {
      if (GameStatsManager.Instance.huntProgress.CurrentActiveMonsterHuntID <= -1)
      {
        if (GameStatsManager.Instance.GetFlag(GungeonFlags.FRIFLE_CORE_HUNTS_COMPLETE) && !GameStatsManager.Instance.GetFlag(GungeonFlags.FRIFLE_REWARD_GREY_MAUSER))
          this.Fsm.Event(this.HuntComplete);
        else
          this.Fsm.Event(this.NeedsNewHunt);
      }
      else if (GameStatsManager.Instance.huntProgress.CurrentActiveMonsterHuntProgress >= GameStatsManager.Instance.huntProgress.ActiveQuest.NumberKillsRequired)
        this.Fsm.Event(this.HuntComplete);
      else
        this.Fsm.Event(this.HuntIncomplete);
    }
  }
}
