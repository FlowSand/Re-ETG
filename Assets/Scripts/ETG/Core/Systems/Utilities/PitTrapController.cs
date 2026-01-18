using UnityEngine;

using Dungeonator;

#nullable disable

public class PitTrapController : BasicTrapController
    {
        protected override void OnDestroy() => base.OnDestroy();

        public override GameObject InstantiateObject(
            RoomHandler targetRoom,
            IntVector2 loc,
            bool deferConfiguration = false)
        {
            IntVector2 intVector2 = loc + targetRoom.area.basePosition;
            for (int x = intVector2.x; x < intVector2.x + this.placeableWidth; ++x)
            {
                for (int y = intVector2.y; y < intVector2.y + this.placeableHeight; ++y)
                {
                    CellData cellData = GameManager.Instance.Dungeon.data.cellData[x][y];
                    cellData.type = CellType.PIT;
                    cellData.fallingPrevented = true;
                }
            }
            return base.InstantiateObject(targetRoom, loc, deferConfiguration);
        }

        protected override void BeginState(BasicTrapController.State newState)
        {
            switch (newState)
            {
                case BasicTrapController.State.Active:
                    for (int x = this.m_cachedPosition.x; x < this.m_cachedPosition.x + this.placeableWidth; ++x)
                    {
                        for (int y = this.m_cachedPosition.y; y < this.m_cachedPosition.y + this.placeableHeight; ++y)
                            GameManager.Instance.Dungeon.data.cellData[x][y].fallingPrevented = false;
                    }
                    if ((bool) (Object) this.specRigidbody)
                    {
                        this.specRigidbody.enabled = false;
                        break;
                    }
                    break;
                case BasicTrapController.State.Resetting:
                    for (int x = this.m_cachedPosition.x; x < this.m_cachedPosition.x + this.placeableWidth; ++x)
                    {
                        for (int y = this.m_cachedPosition.y; y < this.m_cachedPosition.y + this.placeableHeight; ++y)
                            GameManager.Instance.Dungeon.data.cellData[x][y].fallingPrevented = true;
                    }
                    if ((bool) (Object) this.specRigidbody)
                    {
                        this.specRigidbody.enabled = true;
                        break;
                    }
                    break;
            }
            base.BeginState(newState);
        }
    }

