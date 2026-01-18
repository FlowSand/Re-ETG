using Dungeonator;
using Pathfinding;
using UnityEngine;

#nullable disable

public class OccupiedCells
  {
    protected RoomHandler m_cachedRoom;
    protected SpeculativeRigidbody m_specRigidbody;
    protected PixelCollider m_pixelCollider;
    protected bool m_usesCustom;
    protected IntVector2 m_customBasePosition;
    protected IntVector2 m_customDimensions;

    public OccupiedCells(SpeculativeRigidbody specRigidbody, RoomHandler room)
      : this(specRigidbody, specRigidbody.PrimaryPixelCollider, room)
    {
    }

    public OccupiedCells(
      SpeculativeRigidbody specRigidbody,
      PixelCollider pixelCollider,
      RoomHandler room)
    {
      this.m_specRigidbody = specRigidbody;
      this.m_pixelCollider = pixelCollider;
      this.m_cachedRoom = room;
      if (this.m_cachedRoom == null)
        Debug.LogError((object) $"error on: {this.m_specRigidbody.name}{(object) this.m_specRigidbody.transform.position}");
      Pathfinder.Instance.RegisterObstacle(this, this.m_cachedRoom);
    }

    public OccupiedCells(IntVector2 basePosition, IntVector2 dimensions, RoomHandler room)
    {
      this.m_usesCustom = true;
      this.m_customBasePosition = basePosition;
      this.m_customDimensions = dimensions;
      this.m_cachedRoom = room;
      if (this.m_cachedRoom == null)
        Debug.LogError((object) $"error on: {this.m_specRigidbody.name}{(object) this.m_specRigidbody.transform.position}");
      Pathfinder.Instance.RegisterObstacle(this, this.m_cachedRoom);
    }

    public void Clear()
    {
      if (GameManager.HasInstance && !GameManager.Instance.IsLoadingLevel && PhysicsEngine.HasInstance && (bool) (Object) this.m_specRigidbody && (bool) (Object) GameManager.Instance.Dungeon)
      {
        if (this.m_usesCustom)
        {
          RoomHandler absoluteRoom = this.m_customBasePosition.ToVector3().GetAbsoluteRoom();
          if (absoluteRoom != null)
            this.m_cachedRoom = absoluteRoom;
        }
        else
        {
          RoomHandler roomFromPosition = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.m_specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
          if (roomFromPosition != null)
            this.m_cachedRoom = roomFromPosition;
        }
      }
      if (!Pathfinder.HasInstance || this.m_cachedRoom == null)
        return;
      Pathfinder.Instance.DeregisterObstacle(this, this.m_cachedRoom);
    }

    public void FlagCells()
    {
      if (!GameManager.HasInstance || (Object) GameManager.Instance.Dungeon == (Object) null || GameManager.Instance.Dungeon.data == null)
        return;
      DungeonData data = GameManager.Instance.Dungeon.data;
      if (this.m_usesCustom)
      {
        IntVector2 customBasePosition = this.m_customBasePosition;
        IntVector2 intVector2 = this.m_customBasePosition + this.m_customDimensions;
        for (int x = customBasePosition.x; x < intVector2.x; ++x)
        {
          for (int y = customBasePosition.y; y < intVector2.y; ++y)
          {
            CellData cellData = data.cellData[x][y];
            if (cellData != null)
              cellData.isOccupied = true;
          }
        }
      }
      else
      {
        IntVector2 intVector2_1 = this.m_pixelCollider.UnitBottomLeft.ToIntVector2(VectorConversions.Floor);
        IntVector2 intVector2_2 = this.m_pixelCollider.UnitTopRight.ToIntVector2(VectorConversions.Ceil);
        for (int x = intVector2_1.x; x < intVector2_2.x; ++x)
        {
          for (int y = intVector2_1.y; y < intVector2_2.y; ++y)
          {
            CellData cellData = data.cellData[x][y];
            if (cellData != null)
              cellData.isOccupied = true;
          }
        }
      }
    }

    public void UpdateCells() => Pathfinder.Instance.FlagRoomAsDirty(this.m_cachedRoom);
  }

