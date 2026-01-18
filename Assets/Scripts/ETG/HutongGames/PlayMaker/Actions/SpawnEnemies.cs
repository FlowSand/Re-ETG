#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(".NPCs")]
  [Tooltip("Spawns enemies in the NPC's current room.")]
  public class SpawnEnemies : FsmStateAction
  {
    [Tooltip("Type of enemy spawn.")]
    public SpawnEnemies.Type type;
    public RoomEventTriggerCondition roomEventTrigger;
    public bool InstantReinforcement;

    public override void Reset()
    {
      this.type = SpawnEnemies.Type.Reinforcement;
      this.roomEventTrigger = RoomEventTriggerCondition.NPC_TRIGGER_A;
    }

    public override void OnEnter()
    {
      TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
      if (this.type == SpawnEnemies.Type.Reinforcement)
      {
        component.ParentRoom.TriggerReinforcementLayersOnEvent(this.roomEventTrigger, this.InstantReinforcement);
        component.ParentRoom.SealRoom();
      }
      this.Finish();
    }

    public enum Type
    {
      Reinforcement,
    }
  }
}
