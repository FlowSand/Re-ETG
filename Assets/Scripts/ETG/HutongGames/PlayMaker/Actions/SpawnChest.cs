using UnityEngine;

using Dungeonator;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(".NPCs")]
    [HutongGames.PlayMaker.Tooltip("Spawns a chest in the NPC's current room.")]
    public class SpawnChest : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Type of chest to spawn.")]
        public SpawnChest.Type type;
        [HutongGames.PlayMaker.Tooltip("Specific chest to spawn.")]
        public GameObject CustomChest;
        [HutongGames.PlayMaker.Tooltip("Where to spawn the item at.")]
        public SpawnChest.SpawnLocation spawnLocation;
        [HutongGames.PlayMaker.Tooltip("Offset from the TalkDoer to spawn the item at.")]
        public Vector2 spawnOffset;

        public override void Reset()
        {
            this.type = SpawnChest.Type.RoomReward;
            this.CustomChest = (GameObject) null;
        }

        public override void OnEnter()
        {
            TalkDoerLite component = this.Owner.GetComponent<TalkDoerLite>();
            WeightedGameObjectCollection chestCollection = (WeightedGameObjectCollection) null;
            if (this.type == SpawnChest.Type.RoomReward)
                chestCollection = (WeightedGameObjectCollection) null;
            else if (this.type == SpawnChest.Type.Custom)
            {
                WeightedGameObject w = new WeightedGameObject();
                w.SetGameObject(this.CustomChest);
                chestCollection = new WeightedGameObjectCollection();
                chestCollection.Add(w);
            }
            RoomHandler parentRoom = component.ParentRoom;
            if (this.spawnLocation == SpawnChest.SpawnLocation.BestRoomLocation)
                parentRoom.SpawnRoomRewardChest(chestCollection, component.ParentRoom.GetBestRewardLocation(new IntVector2(2, 1)));
            else if (this.spawnLocation == SpawnChest.SpawnLocation.OffsetFromTalkDoer)
            {
                Vector2 idealPoint = (!((Object) component.specRigidbody != (Object) null) ? component.sprite.WorldCenter : component.specRigidbody.UnitCenter) + this.spawnOffset;
                parentRoom.SpawnRoomRewardChest(chestCollection, component.ParentRoom.GetBestRewardLocation(new IntVector2(2, 1), idealPoint));
            }
            this.Finish();
        }

        public enum Type
        {
            RoomReward,
            Custom,
        }

        public enum SpawnLocation
        {
            BestRoomLocation,
            OffsetFromTalkDoer,
        }
    }
}
