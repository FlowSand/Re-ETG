#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".Brave")]
  [Tooltip("Sets a flag on the player's save data.")]
  public class SetSaveFlag : FsmStateAction
  {
    [Tooltip("The flag.")]
    public GungeonFlags targetFlag;
    [Tooltip("The value to set the flag to.")]
    public FsmBool value;

    public override string ErrorCheck()
    {
      string empty = string.Empty;
      if (this.targetFlag == GungeonFlags.NONE)
        empty += "Target flag is NONE. This is a mistake.";
      return empty;
    }

    public override void Reset()
    {
      this.targetFlag = GungeonFlags.NONE;
      this.value = (FsmBool) false;
    }

    public override void OnEnter()
    {
      GameStatsManager.Instance.SetFlag(this.targetFlag, this.value.Value);
      this.Finish();
    }
  }
}
