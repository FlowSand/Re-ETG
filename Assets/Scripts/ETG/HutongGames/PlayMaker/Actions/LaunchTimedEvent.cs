using System;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".Brave")]
  public class LaunchTimedEvent : FsmStateAction
  {
    public GungeonFlags targetFlag;
    public float AllotedTime = 60f;

    public override void Reset()
    {
    }

    public override void OnEnter()
    {
      GameManager.Instance.LaunchTimedEvent(this.AllotedTime, (Action<bool>) (a => GameStatsManager.Instance.SetFlag(this.targetFlag, a)));
      this.Finish();
    }
  }
}
