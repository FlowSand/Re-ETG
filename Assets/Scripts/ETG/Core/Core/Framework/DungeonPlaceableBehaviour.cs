// Decompiled with JetBrains decompiler
// Type: DungeonPlaceableBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class DungeonPlaceableBehaviour : BraveBehaviour, IHasDwarfConfigurables
  {
    [SerializeField]
    public int placeableWidth = 1;
    [SerializeField]
    public int placeableHeight = 1;
    [SerializeField]
    public DungeonPlaceableBehaviour.PlaceableDifficulty difficulty;
    [SerializeField]
    public bool isPassable = true;

    public IntVector2 PlacedPosition { get; set; }

    public virtual GameObject InstantiateObjectDirectional(
      RoomHandler targetRoom,
      IntVector2 location,
      DungeonData.Direction direction)
    {
      BraveUtility.Log("Calling InstantiateDirectional on a DungeonPlaceableBehaviour that hasn't implemented it.", Color.yellow, BraveUtility.LogVerbosity.IMPORTANT);
      return DungeonPlaceableUtility.InstantiateDungeonPlaceable(this.gameObject, targetRoom, location, false);
    }

    public virtual GameObject InstantiateObject(
      RoomHandler targetRoom,
      IntVector2 location,
      bool deferConfiguration = false)
    {
      return DungeonPlaceableUtility.InstantiateDungeonPlaceable(this.gameObject, targetRoom, location, deferConfiguration);
    }

    public virtual GameObject InstantiateObjectOnlyActors(
      RoomHandler targetRoom,
      IntVector2 location,
      bool deferConfiguration = false)
    {
      return DungeonPlaceableUtility.InstantiateDungeonPlaceableOnlyActors(this.gameObject, targetRoom, location, deferConfiguration);
    }

    public virtual int GetMinimumDifficulty() => 0;

    public virtual int GetMaximumDifficulty() => 0;

    public virtual int GetWidth() => this.placeableWidth;

    public virtual int GetHeight() => this.placeableHeight;

    protected override void OnDestroy() => base.OnDestroy();

    public RoomHandler GetAbsoluteParentRoom()
    {
      return GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY());
    }

    public void SetAreaPassable()
    {
      for (int x = this.PlacedPosition.x; x < this.PlacedPosition.x + this.placeableWidth; ++x)
      {
        for (int y = this.PlacedPosition.y; y < this.PlacedPosition.y + this.placeableHeight; ++y)
          GameManager.Instance.Dungeon.data[x, y].isOccupied = false;
      }
    }

    public enum PlaceableDifficulty
    {
      BASE,
      HARD,
      HARDER,
      HARDEST,
    }
  }

