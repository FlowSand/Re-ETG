using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

using Dungeonator;

#nullable disable

public class IronCoinItem : PlayerItem
    {
        public float ChanceToTargetBoss = 0.01f;
        public GameObject OnUsedVFX;
        public GameObject NotePrefab;
        public GoopDefinition BloodDefinition;

        protected override void DoEffect(PlayerController user)
        {
            if ((bool) (Object) this.OnUsedVFX)
                user.PlayEffectOnActor(this.OnUsedVFX, new Vector3(0.0f, 0.25f, 0.0f), false);
            List<RoomHandler> roomHandlerList = new List<RoomHandler>();
            bool flag1 = (double) Random.value < (double) this.ChanceToTargetBoss;
            bool flag2 = false;
            for (int index = 0; index < GameManager.Instance.Dungeon.data.rooms.Count; ++index)
            {
                if (GameManager.Instance.Dungeon.data.rooms[index].visibility == RoomHandler.VisibilityStatus.OBSCURED && GameManager.Instance.Dungeon.data.rooms[index].HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
                {
                    if (flag2)
                    {
                        if (GameManager.Instance.Dungeon.data.rooms[index].area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS)
                            roomHandlerList.Add(GameManager.Instance.Dungeon.data.rooms[index]);
                    }
                    else if (GameManager.Instance.Dungeon.data.rooms[index].area.PrototypeRoomCategory != PrototypeDungeonRoom.RoomCategory.BOSS)
                        roomHandlerList.Add(GameManager.Instance.Dungeon.data.rooms[index]);
                }
            }
            if (roomHandlerList.Count <= 0)
                return;
            RoomHandler targetRoom = roomHandlerList[Random.Range(0, roomHandlerList.Count)];
            user.StartCoroutine(this.SlaughterRoom(targetRoom));
        }

        [DebuggerHidden]
        private IEnumerator SlaughterRoom(RoomHandler targetRoom)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new IronCoinItem__SlaughterRoomc__Iterator0()
            {
                targetRoom = targetRoom,
                _this = this
            };
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

