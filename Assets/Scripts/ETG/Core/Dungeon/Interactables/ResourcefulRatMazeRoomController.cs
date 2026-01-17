// Decompiled with JetBrains decompiler
// Type: ResourcefulRatMazeRoomController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class ResourcefulRatMazeRoomController : DungeonPlaceableBehaviour
    {
      private List<ResourcefulRatMazeRoomController> m_mazes;
      private int minX = int.MaxValue;
      private int minY = int.MaxValue;
      private int maxX = int.MinValue;
      private int maxY = int.MinValue;
      private DungeonData.Direction m_correctDirection;
      private RoomHandler m_room;
      private int m_roomIndex;

      [DebuggerHidden]
      public IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ResourcefulRatMazeRoomController.<Start>c__Iterator0()
        {
          _this = this
        };
      }

      private int GetPositionInChain(RoomHandler room)
      {
        List<ResourcefulRatMazeRoomController> list = new List<ResourcefulRatMazeRoomController>((IEnumerable<ResourcefulRatMazeRoomController>) UnityEngine.Object.FindObjectsOfType<ResourcefulRatMazeRoomController>()).OrderBy<ResourcefulRatMazeRoomController, float>((Func<ResourcefulRatMazeRoomController, float>) (a => a.transform.position.x)).ToList<ResourcefulRatMazeRoomController>();
        this.m_mazes = list;
        return list.IndexOf(this);
      }

      public void Initialize()
      {
        RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
        this.m_room = absoluteRoom;
        this.m_roomIndex = this.GetPositionInChain(absoluteRoom);
        this.m_correctDirection = GameManager.GetResourcefulRatSolution()[this.m_roomIndex];
        for (int index = 0; index < absoluteRoom.Cells.Count; ++index)
        {
          IntVector2 cell = absoluteRoom.Cells[index];
          this.minX = Mathf.Min(this.minX, cell.x);
          this.minY = Mathf.Min(this.minY, cell.y);
          this.maxX = Mathf.Max(this.maxX, cell.x);
          this.maxY = Mathf.Max(this.maxY, cell.y);
        }
      }

      private bool PlayerInTargetCell(Vector2 playerPos)
      {
        switch (this.m_correctDirection)
        {
          case DungeonData.Direction.NORTH:
            if ((double) playerPos.y > (double) this.maxY)
              return true;
            break;
          case DungeonData.Direction.EAST:
            if ((double) playerPos.x > (double) this.maxX)
              return true;
            break;
          case DungeonData.Direction.SOUTH:
            if ((double) playerPos.y < (double) (this.minY + 1))
              return true;
            break;
          case DungeonData.Direction.WEST:
            if ((double) playerPos.x < (double) (this.minX + 1))
              return true;
            break;
        }
        return false;
      }

      private bool PlayerInFailCell(Vector2 playerPos)
      {
        bool flag = this.m_roomIndex != 0;
        switch (this.m_correctDirection)
        {
          case DungeonData.Direction.NORTH:
            if ((double) playerPos.x > (double) this.maxX || (double) playerPos.y < (double) (this.minY + 1) || flag && (double) playerPos.x < (double) (this.minX + 1))
              return true;
            break;
          case DungeonData.Direction.EAST:
            if ((double) playerPos.y > (double) this.maxY || (double) playerPos.y < (double) (this.minY + 1) || flag && (double) playerPos.x < (double) (this.minX + 1))
              return true;
            break;
          case DungeonData.Direction.SOUTH:
            if ((double) playerPos.x > (double) this.maxX || (double) playerPos.y > (double) this.maxY || flag && (double) playerPos.x < (double) (this.minX + 1))
              return true;
            break;
          case DungeonData.Direction.WEST:
            if ((double) playerPos.x > (double) this.maxX || (double) playerPos.y > (double) this.maxY || (double) playerPos.y < (double) (this.minY + 1))
              return true;
            break;
        }
        return false;
      }

      public void Update()
      {
        for (int index1 = 0; index1 < GameManager.Instance.AllPlayers.Length; ++index1)
        {
          PlayerController allPlayer = GameManager.Instance.AllPlayers[index1];
          Vector2 spriteBottomCenter = (Vector2) allPlayer.SpriteBottomCenter;
          if (allPlayer.CurrentRoom == this.m_room)
          {
            if (this.PlayerInTargetCell(spriteBottomCenter))
            {
              if (this.m_mazes.IndexOf(this) == this.m_mazes.Count - 1)
              {
                RoomHandler roomHandler = (RoomHandler) null;
                foreach (RoomHandler room in GameManager.Instance.Dungeon.data.rooms)
                {
                  if (room.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS)
                  {
                    roomHandler = room;
                    break;
                  }
                }
                for (int index2 = 0; index2 < roomHandler.connectedRooms.Count; ++index2)
                {
                  if (roomHandler.connectedRooms[index2].distanceFromEntrance <= roomHandler.distanceFromEntrance)
                  {
                    roomHandler = roomHandler.connectedRooms[index2];
                    break;
                  }
                }
                allPlayer.WarpToPoint(roomHandler.Epicenter.ToVector2());
              }
              else
              {
                Vector2 targetPoint = this.m_mazes[this.m_mazes.IndexOf(this) + 1].transform.position.XY();
                allPlayer.WarpToPoint(targetPoint);
              }
            }
            else if (this.PlayerInFailCell(spriteBottomCenter))
            {
              RoomHandler roomHandler = (RoomHandler) null;
              foreach (RoomHandler room in GameManager.Instance.Dungeon.data.rooms)
              {
                if (room.area.PrototypeRoomName.Contains("FailRoom"))
                {
                  roomHandler = room;
                  break;
                }
              }
              allPlayer.WarpToPoint(roomHandler.Epicenter.ToVector2());
            }
          }
        }
      }
    }

}
