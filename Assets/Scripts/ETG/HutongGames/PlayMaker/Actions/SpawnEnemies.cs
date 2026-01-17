// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.SpawnEnemies
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
