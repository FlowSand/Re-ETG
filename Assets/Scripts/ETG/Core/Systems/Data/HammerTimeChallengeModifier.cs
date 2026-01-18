using Dungeonator;
using UnityEngine;

#nullable disable

public class HammerTimeChallengeModifier : ChallengeModifier
  {
    public DungeonPlaceable HammerPlaceable;
    public float MinTimeBetweenAttacks = 3f;
    public float MaxTimeBetweenAttacks = 3f;

    public override bool IsValid(RoomHandler room)
    {
      GlobalDungeonData.ValidTilesets tilesetId = GameManager.Instance.Dungeon.tileIndices.tilesetId;
      switch (tilesetId)
      {
        case GlobalDungeonData.ValidTilesets.GUNGEON:
        case GlobalDungeonData.ValidTilesets.CASTLEGEON:
          return true;
        default:
          return tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON;
      }
    }

    private void Start()
    {
      RoomHandler currentRoom = GameManager.Instance.PrimaryPlayer.CurrentRoom;
      GameObject gameObject = this.HammerPlaceable.InstantiateObject(currentRoom, currentRoom.Epicenter);
      if (!(bool) (Object) gameObject)
        return;
      ForgeHammerController component = gameObject.GetComponent<ForgeHammerController>();
      if (!(bool) (Object) component)
        return;
      component.MinTimeBetweenAttacks = this.MinTimeBetweenAttacks;
      component.MaxTimeBetweenAttacks = this.MaxTimeBetweenAttacks;
    }
  }

