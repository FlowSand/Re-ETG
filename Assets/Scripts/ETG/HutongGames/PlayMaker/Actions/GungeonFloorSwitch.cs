#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Sends an Event based on the current floor.")]
  [ActionCategory(ActionCategory.Logic)]
  public class GungeonFloorSwitch : FsmStateAction
  {
    public bool DoSendEvent = true;
    public bool ChangeVariable;
    public GlobalDungeonData.ValidTilesets[] compareTo;
    public FsmEvent[] sendEvent;
    public GlobalDungeonData.ValidTilesets[] varCompareTo;
    public FsmString[] targetStrings;
    public FsmString targetVariable;
    public bool everyFrame;

    public override void Reset()
    {
      this.compareTo = new GlobalDungeonData.ValidTilesets[1];
      this.sendEvent = new FsmEvent[1];
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      if (this.DoSendEvent)
        this.DoFloorSwitch();
      if (this.ChangeVariable)
        this.DoVariableSwitch();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate()
    {
      if (this.DoSendEvent)
        this.DoFloorSwitch();
      if (!this.ChangeVariable)
        return;
      this.DoVariableSwitch();
    }

    private void DoVariableSwitch()
    {
      for (int index = 0; index < this.varCompareTo.Length; ++index)
      {
        if (GameManager.Instance.Dungeon.tileIndices.tilesetId == this.varCompareTo[index])
        {
          this.targetVariable.Value = this.targetStrings[index].Value;
          break;
        }
      }
      this.Finish();
    }

    private void DoFloorSwitch()
    {
      for (int index = 0; index < this.compareTo.Length; ++index)
      {
        if (GameManager.Instance.Dungeon.tileIndices.tilesetId == this.compareTo[index])
        {
          this.Fsm.Event(this.sendEvent[index]);
          break;
        }
      }
    }
  }
}
