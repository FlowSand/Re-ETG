using System;
using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class RideInCartsBehavior : MovementBehaviorBase
    {
        private MineCartController m_currentTarget;
        private bool m_ridingCart;
        protected float m_findNewCartTimer;

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_findNewCartTimer);
        }

        private MineCartController GetAvailableMineCart()
        {
            List<MineCartController> componentsInRoom = this.m_aiActor.ParentRoom.GetComponentsInRoom<MineCartController>();
            for (int index = 0; index < componentsInRoom.Count; ++index)
            {
                if (componentsInRoom[index].IsOnlyPlayerMinecart || componentsInRoom[index].occupation != MineCartController.CartOccupationState.EMPTY)
                {
                    componentsInRoom.RemoveAt(index);
                    --index;
                }
            }
            componentsInRoom.Sort((Comparison<MineCartController>) ((a, b) => Vector2.Distance(this.m_aiActor.CenterPosition, a.sprite.WorldCenter).CompareTo(Vector2.Distance(this.m_aiActor.CenterPosition, b.sprite.WorldCenter))));
            return componentsInRoom.Count == 0 ? (MineCartController) null : componentsInRoom[0];
        }

        public override BehaviorResult Update()
        {
            if (GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.MINEGEON)
                return BehaviorResult.Continue;
            if (this.m_ridingCart)
                return BehaviorResult.SkipRemainingClassBehaviors;
            if ((double) this.m_findNewCartTimer <= 0.0)
            {
                this.m_currentTarget = this.GetAvailableMineCart();
                this.m_findNewCartTimer = 5f;
            }
            if (!((UnityEngine.Object) this.m_currentTarget != (UnityEngine.Object) null))
                return BehaviorResult.Continue;
            if (this.m_currentTarget.occupation != MineCartController.CartOccupationState.EMPTY)
            {
                this.m_findNewCartTimer = 0.0f;
                return BehaviorResult.Continue;
            }
            this.m_aiActor.PathfindToPosition(this.m_currentTarget.sprite.WorldCenter);
            if ((double) Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, this.m_currentTarget.specRigidbody.UnitCenter) < 5.0 && (double) BraveMathCollege.DistBetweenRectangles(this.m_aiActor.specRigidbody.UnitBottomLeft, this.m_aiActor.specRigidbody.UnitDimensions, this.m_currentTarget.specRigidbody.UnitBottomLeft, this.m_currentTarget.specRigidbody.UnitDimensions) < 0.5)
            {
                this.m_aiActor.ClearPath();
                this.m_currentTarget.BecomeOccupied(this.m_aiActor);
                this.m_ridingCart = true;
            }
            return BehaviorResult.SkipRemainingClassBehaviors;
        }
    }

