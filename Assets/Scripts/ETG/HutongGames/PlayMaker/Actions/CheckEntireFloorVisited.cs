using Dungeonator;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    public class CheckEntireFloorVisited : FsmStateAction
    {
        public static bool IsQuestCallbackActive;
        [HutongGames.PlayMaker.Tooltip("Event sent if there are.")]
        public FsmEvent HasVisited;
        [HutongGames.PlayMaker.Tooltip("Event sent if there aren't.")]
        public FsmEvent HasNotVisited;
        public FsmBool IncludeSecretRooms = (FsmBool) false;
        public FsmBool IncludeWarpRooms = (FsmBool) false;
        public FsmBool OnlyIncludeStandardRooms = (FsmBool) true;

        public override void Awake() => base.Awake();

        public override void OnEnter()
        {
            if (this.TestCompletion())
            {
                if (CheckEntireFloorVisited.IsQuestCallbackActive)
                {
                    GameManager.Instance.Dungeon.OnAnyRoomVisited -= new System.Action(this.NotifyComplete);
                    CheckEntireFloorVisited.IsQuestCallbackActive = false;
                }
                this.Fsm.Event(this.HasVisited);
            }
            else
            {
                if (!CheckEntireFloorVisited.IsQuestCallbackActive)
                {
                    GameManager.Instance.Dungeon.OnAnyRoomVisited += new System.Action(this.NotifyComplete);
                    CheckEntireFloorVisited.IsQuestCallbackActive = true;
                }
                this.Fsm.Event(this.HasNotVisited);
            }
            this.Finish();
        }

        private bool TestCompletion()
        {
            bool flag1 = GameManager.Instance.Dungeon.AllRoomsVisited;
            if (!this.IncludeSecretRooms.Value || !this.IncludeWarpRooms.Value || this.OnlyIncludeStandardRooms.Value)
            {
                flag1 = true;
                for (int index = 0; index < GameManager.Instance.Dungeon.data.rooms.Count; ++index)
                {
                    RoomHandler room = GameManager.Instance.Dungeon.data.rooms[index];
                    bool isSecretRoom = room.IsSecretRoom;
                    bool isStartOfWarpWing = room.IsStartOfWarpWing;
                    bool flag2 = room.visibility == RoomHandler.VisibilityStatus.OBSCURED;
                    bool flag3 = room.IsStandardRoom || room.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SPECIAL || room.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.REWARD;
                    if (!(bool) (UnityEngine.Object) room.OverrideTilemap && flag2 && (this.IncludeSecretRooms.Value || !isSecretRoom) && (this.IncludeWarpRooms.Value || !isStartOfWarpWing) && (!this.OnlyIncludeStandardRooms.Value || flag3) && !room.RevealedOnMap)
                    {
                        flag1 = false;
                        break;
                    }
                }
            }
            return flag1;
        }

        private void NotifyComplete()
        {
            if (!this.TestCompletion())
                return;
            CheckEntireFloorVisited.IsQuestCallbackActive = false;
            GameManager.Instance.Dungeon.OnAnyRoomVisited -= new System.Action(this.NotifyComplete);
            GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.GetString("#LOSTADVENTURER_NOTIFICATION_HEADER"), StringTableManager.GetString("#LOSTADVENTURER_NOTIFICATION_BODY"), this.Owner.GetComponent<TalkDoerLite>().OptionalCustomNotificationSprite.Collection, this.Owner.GetComponent<TalkDoerLite>().OptionalCustomNotificationSprite.spriteId, UINotificationController.NotificationColor.GOLD);
        }
    }
}
