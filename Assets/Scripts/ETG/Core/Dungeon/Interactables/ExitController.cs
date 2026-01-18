using UnityEngine;

using Dungeonator;

#nullable disable

public class ExitController : DungeonPlaceableBehaviour, IPlaceConfigurable
    {
        public int wallClearanceXStart = 1;
        public int wallClearanceYStart = 1;
        public int wallClearanceWidth = 4;
        public int wallClearanceHeight = 4;

        private void Start()
        {
            SpeculativeRigidbody componentInChildren = this.GetComponentInChildren<SpeculativeRigidbody>();
            if (!(bool) (Object) componentInChildren)
                return;
            componentInChildren.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
        }

        public void ConfigureOnPlacement(RoomHandler room)
        {
            IntVector2 intVector2 = this.transform.position.IntXY(VectorConversions.Floor);
            for (int wallClearanceXstart = this.wallClearanceXStart; wallClearanceXstart < this.wallClearanceWidth + this.wallClearanceXStart; ++wallClearanceXstart)
            {
                for (int wallClearanceYstart = this.wallClearanceYStart; wallClearanceYstart < this.wallClearanceHeight + this.wallClearanceYStart; ++wallClearanceYstart)
                {
                    CellData cellData = GameManager.Instance.Dungeon.data[intVector2 + new IntVector2(wallClearanceXstart, wallClearanceYstart)];
                    cellData.cellVisualData.containsObjectSpaceStamp = true;
                    cellData.cellVisualData.containsWallSpaceStamp = true;
                    cellData.cellVisualData.shouldIgnoreWallDrawing = true;
                }
            }
        }

        protected virtual void OnRigidbodyCollision(CollisionData rigidbodyCollision)
        {
            if (!((Object) rigidbodyCollision.OtherRigidbody.GetComponent<PlayerController>() != (Object) null))
                return;
            Pixelator.Instance.FadeToBlack(0.5f);
            GameManager.Instance.DelayedLoadNextLevel(0.5f);
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

