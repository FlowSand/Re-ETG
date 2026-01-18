using UnityEngine;

using Dungeonator;

#nullable disable

public class GoopPlaceable : DungeonPlaceableBehaviour, IPlaceConfigurable
    {
        public GoopDefinition goop;
        [DwarfConfigurable]
        public float radius = 1f;
        private RoomHandler m_room;

        protected override void OnDestroy()
        {
            if (this.m_room != null)
                this.m_room.Entered -= new RoomHandler.OnEnteredEventHandler(this.PlayerEntered);
            base.OnDestroy();
        }

        public void ConfigureOnPlacement(RoomHandler room)
        {
            this.m_room = room;
            this.m_room.Entered += new RoomHandler.OnEnteredEventHandler(this.PlayerEntered);
        }

        public void PlayerEntered(PlayerController playerController)
        {
            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goop).AddGoopCircle(this.transform.position.XY() + new Vector2(0.5f, 0.5f), this.radius);
            if (this.m_room == null)
                return;
            this.m_room.Entered -= new RoomHandler.OnEnteredEventHandler(this.PlayerEntered);
        }
    }

