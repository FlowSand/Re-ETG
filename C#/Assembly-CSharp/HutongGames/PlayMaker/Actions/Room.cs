// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.Room
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[Tooltip("Respondes to chest events.")]
[ActionCategory(".NPCs")]
public class Room : FsmStateAction
{
  [Tooltip("Seals the room the Owner is in.")]
  public FsmBool seal;
  [Tooltip("Unseals the room the Owner is in.")]
  public FsmBool unseal;
  [Tooltip("Ignores SealPrior in Tutorial.")]
  public FsmBool unsealAllForceTutorial;

  public override void Reset()
  {
    this.seal = (FsmBool) false;
    this.unseal = (FsmBool) false;
  }

  public override void OnEnter()
  {
    TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
    component.specRigidbody.Initialize();
    RoomHandler roomFromPosition = GameManager.Instance.Dungeon.GetRoomFromPosition(component.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
    if (this.seal.Value)
    {
      roomFromPosition.npcSealState = RoomHandler.NPCSealState.SealAll;
      if (GameManager.Instance.InTutorial && component.name.Contains("NPC_Tutorial_Knight_001_intro"))
        roomFromPosition.npcSealState = RoomHandler.NPCSealState.SealNext;
      roomFromPosition.SealRoom();
    }
    else if (this.unseal.Value)
    {
      roomFromPosition.npcSealState = RoomHandler.NPCSealState.SealNone;
      if (GameManager.Instance.InTutorial)
        roomFromPosition.npcSealState = !component.name.Contains("NPC_Tutorial_Knight_001_intro") ? RoomHandler.NPCSealState.SealPrior : RoomHandler.NPCSealState.SealNone;
      if (this.unsealAllForceTutorial.Value)
        roomFromPosition.npcSealState = RoomHandler.NPCSealState.SealNone;
      roomFromPosition.UnsealRoom();
    }
    this.Finish();
  }
}
