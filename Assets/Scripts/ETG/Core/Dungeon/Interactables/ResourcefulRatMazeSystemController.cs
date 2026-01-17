// Decompiled with JetBrains decompiler
// Type: ResourcefulRatMazeSystemController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    public class ResourcefulRatMazeSystemController : DungeonPlaceableBehaviour
    {
      private List<RoomHandler> m_centralRoomSeries;
      private bool m_playerHasLeftEntrance;
      private DungeonData.Direction[] m_currentSolution;
      private int m_currentTargetDirectionIndex;
      private int m_currentLivingRoomIndex;
      private int m_sequentialErrors;
      private int m_errors;
      private bool m_mazeIsActive;
      private bool m_isInitialized;

      [DebuggerHidden]
      public IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ResourcefulRatMazeSystemController__Startc__Iterator0()
        {
          _this = this
        };
      }

      private void ResetMaze()
      {
        UnityEngine.Debug.LogError((object) "resetting the maze!!!!");
        this.m_currentTargetDirectionIndex = 0;
        this.m_currentLivingRoomIndex = 0;
        this.m_sequentialErrors = 0;
        this.m_errors = 0;
        this.m_playerHasLeftEntrance = false;
        this.m_mazeIsActive = true;
      }

      public void Initialize()
      {
        if (this.m_isInitialized)
          return;
        RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
        this.m_centralRoomSeries = new List<RoomHandler>();
        this.m_centralRoomSeries.Add(absoluteRoom);
        absoluteRoom.OverrideVisibility = RoomHandler.VisibilityStatus.CURRENT;
        double num = (double) Pixelator.Instance.ProcessOcclusionChange(IntVector2.Zero, 1f, absoluteRoom, false);
        this.m_currentSolution = GameManager.GetResourcefulRatSolution();
        if (GameStatsManager.Instance.GetFlag(GungeonFlags.RESOURCEFUL_RAT_NOTE_06))
        {
          StringBuilder message = new StringBuilder("Rat Solution: ");
          foreach (DungeonData.Direction direction in this.m_currentSolution)
            message.Append((object) direction).Append(" ");
          UnityEngine.Debug.LogError((object) message);
        }
        PlayerController bestActivePlayer = GameManager.Instance.BestActivePlayer;
        for (int index = 0; index < absoluteRoom.connectedRooms.Count; ++index)
          absoluteRoom.connectedRooms[index].PreventRevealEver = true;
        for (int index1 = 0; index1 < GameManager.Instance.Dungeon.data.rooms.Count; ++index1)
        {
          RoomHandler room = GameManager.Instance.Dungeon.data.rooms[index1];
          if (room.connectedRooms.Count == 1 && room.connectedRooms[0] == GameManager.Instance.Dungeon.data.Entrance)
          {
            room.TargetPitfallRoom = absoluteRoom;
            room.ForcePitfallForFliers = true;
          }
          if (room.area.PrototypeRoomSpecialSubcategory == PrototypeDungeonRoom.RoomSpecialSubCategory.NPC_STORY && room != absoluteRoom && !this.m_centralRoomSeries.Contains(room))
          {
            this.m_centralRoomSeries.Add(room);
            for (int index2 = 0; index2 < room.connectedRooms.Count; ++index2)
              room.connectedRooms[index2].PreventRevealEver = true;
          }
        }
        this.m_mazeIsActive = true;
        this.m_isInitialized = true;
      }

      private void Update()
      {
        if (Dungeon.IsGenerating || !GameManager.HasInstance || (Object) GameManager.Instance.Dungeon == (Object) null)
          return;
        if (!this.m_isInitialized)
          this.Initialize();
        PlayerController bestActivePlayer = GameManager.Instance.BestActivePlayer;
        if (this.m_playerHasLeftEntrance && (Object) bestActivePlayer != (Object) null && bestActivePlayer.CurrentRoom == GameManager.Instance.Dungeon.data.Entrance)
        {
          this.ResetMaze();
        }
        else
        {
          if (!(bool) (Object) bestActivePlayer || bestActivePlayer.IsGhost || (bool) (Object) bestActivePlayer && (bool) (Object) bestActivePlayer.healthHaver && bestActivePlayer.healthHaver.IsDead || !this.m_mazeIsActive)
            return;
          if (!this.m_playerHasLeftEntrance)
          {
            if (!(bool) (Object) bestActivePlayer || bestActivePlayer.CurrentRoom == null || bestActivePlayer.CurrentRoom != this.m_centralRoomSeries[0])
              return;
            this.m_playerHasLeftEntrance = true;
          }
          else
          {
            PlayerController cp = bestActivePlayer;
            RoomHandler roomHandler = this.m_centralRoomSeries[this.m_currentLivingRoomIndex];
            DungeonData.Direction directionFromVector2 = DungeonData.GetDirectionFromVector2(BraveUtility.GetMajorAxis(cp.CenterPosition - roomHandler.Epicenter.ToVector2()));
            if (cp.CurrentRoom == roomHandler || cp.InExitCell)
              return;
            if (this.m_errors < 2 && directionFromVector2 == this.m_currentSolution[this.m_currentTargetDirectionIndex])
            {
              if (this.m_currentTargetDirectionIndex == 5)
              {
                this.HandleVictory(cp);
                this.m_mazeIsActive = false;
              }
              else
              {
                int newLivingRoomIndex = 0;
                if (this.m_currentLivingRoomIndex < 5)
                  newLivingRoomIndex = this.m_currentLivingRoomIndex + 1;
                else if (this.m_currentLivingRoomIndex == 6)
                  newLivingRoomIndex = 1;
                this.HandleContinuance(cp, newLivingRoomIndex);
                ++this.m_currentTargetDirectionIndex;
              }
            }
            else if (this.m_errors >= 2)
            {
              this.HandleFailure(cp);
              this.m_mazeIsActive = false;
            }
            else
            {
              this.HandleTemporaryFailure(cp);
              ++this.m_errors;
            }
          }
        }
      }

      private void HandleVictory(PlayerController cp)
      {
        RoomHandler newRoom = (RoomHandler) null;
        foreach (RoomHandler room in GameManager.Instance.Dungeon.data.rooms)
        {
          if (room.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS)
          {
            newRoom = room;
            break;
          }
        }
        for (int index = 0; index < newRoom.connectedRooms.Count; ++index)
        {
          if (newRoom.connectedRooms[index].distanceFromEntrance <= newRoom.distanceFromEntrance)
          {
            newRoom = newRoom.connectedRooms[index];
            break;
          }
        }
        for (int index = 0; index < newRoom.connectedRooms.Count; ++index)
        {
          if (newRoom.connectedRooms[index].distanceFromEntrance <= newRoom.distanceFromEntrance)
          {
            newRoom = newRoom.connectedRooms[index];
            break;
          }
        }
        Vector2 vector2_1 = GameManager.Instance.MainCameraController.transform.position.XY() - cp.transform.position.XY();
        Vector2 vector2_2 = cp.transform.position.XY() - cp.CurrentRoom.area.basePosition.ToVector2();
        Vector2 targetPoint = newRoom.area.basePosition.ToVector2() + vector2_2 + new Vector2(3f, 3f);
        cp.WarpToPointAndBringCoopPartner(targetPoint, doFollowers: true);
        cp.ForceChangeRoom(newRoom);
        GameManager.Instance.MainCameraController.transform.position = (cp.transform.position.XY() + vector2_1).ToVector3ZUp(GameManager.Instance.MainCameraController.transform.position.z);
      }

      private void HandleContinuance(PlayerController cp, int newLivingRoomIndex)
      {
        int currentLivingRoomIndex = this.m_currentLivingRoomIndex;
        this.m_currentLivingRoomIndex = newLivingRoomIndex;
        Vector2 vector2_1 = GameManager.Instance.MainCameraController.transform.position.XY() - cp.transform.position.XY();
        Vector2 vector2_2 = cp.transform.position.XY() - cp.CurrentRoom.area.basePosition.ToVector2();
        Vector2 targetPoint = this.m_centralRoomSeries[this.m_currentLivingRoomIndex].area.basePosition.ToVector2() + vector2_2;
        cp.WarpToPointAndBringCoopPartner(targetPoint, doFollowers: true);
        cp.ForceChangeRoom(this.m_centralRoomSeries[this.m_currentLivingRoomIndex]);
        this.HandleNearestExitOcclusion(cp);
        GameManager.Instance.MainCameraController.transform.position = (cp.transform.position.XY() + vector2_1).ToVector3ZUp(GameManager.Instance.MainCameraController.transform.position.z);
      }

      private void HandleNearestExitOcclusion(PlayerController cp)
      {
        RuntimeExitDefinition runtimeExitDefinition = (RuntimeExitDefinition) null;
        float num = float.MaxValue;
        for (int index = 0; index < cp.CurrentRoom.area.instanceUsedExits.Count; ++index)
        {
          RuntimeRoomExitData exitToLocalData = cp.CurrentRoom.area.exitToLocalDataMap[cp.CurrentRoom.area.instanceUsedExits[index]];
          float magnitude = ((cp.CurrentRoom.area.basePosition + exitToLocalData.ExitOrigin - IntVector2.One).ToCenterVector2() - cp.CenterPosition).magnitude;
          if ((double) magnitude < (double) num && cp.CurrentRoom.exitDefinitionsByExit.ContainsKey(exitToLocalData))
          {
            num = magnitude;
            runtimeExitDefinition = cp.CurrentRoom.exitDefinitionsByExit[exitToLocalData];
          }
        }
        if (runtimeExitDefinition == null)
          return;
        IntVector2 intVector2_1 = IntVector2.MaxValue;
        IntVector2 intVector2_2 = IntVector2.MinValue;
        DungeonData data = GameManager.Instance.Dungeon.data;
        foreach (IntVector2 intVector2_3 in runtimeExitDefinition.GetCellsForRoom(runtimeExitDefinition.downstreamRoom))
        {
          this.ProcessCell(data, intVector2_3);
          intVector2_1 = IntVector2.Min(intVector2_1, intVector2_3);
          intVector2_2 = IntVector2.Max(intVector2_2, intVector2_3 + new IntVector2(0, 2));
        }
        foreach (IntVector2 intVector2_4 in runtimeExitDefinition.GetCellsForRoom(runtimeExitDefinition.upstreamRoom))
        {
          this.ProcessCell(data, intVector2_4);
          intVector2_1 = IntVector2.Min(intVector2_1, intVector2_4);
          intVector2_2 = IntVector2.Max(intVector2_2, intVector2_4 + new IntVector2(0, 2));
        }
        foreach (IntVector2 intermediaryCell in runtimeExitDefinition.IntermediaryCells)
        {
          this.ProcessCell(data, intermediaryCell);
          intVector2_1 = IntVector2.Min(intVector2_1, intermediaryCell);
          intVector2_2 = IntVector2.Max(intVector2_2, intermediaryCell + new IntVector2(0, 2));
        }
        Pixelator.Instance.ProcessModifiedRanges(intVector2_1, intVector2_2);
        Pixelator.Instance.MarkOcclusionDirty();
      }

      private void ProcessCell(DungeonData data, IntVector2 pos)
      {
        CellData cellData1 = data[pos];
        if (cellData1 != null)
        {
          cellData1.occlusionData.cellRoomVisiblityCount = Mathf.RoundToInt(Mathf.Clamp01(1f));
          cellData1.occlusionData.cellRoomVisitedCount = Mathf.RoundToInt(Mathf.Clamp01(1f));
          cellData1.occlusionData.cellVisibleTargetOcclusion = 0.0f;
          cellData1.occlusionData.cellVisitedTargetOcclusion = 0.7f;
          cellData1.occlusionData.overrideOcclusion = true;
          cellData1.occlusionData.cellOcclusion = 0.0f;
          BraveUtility.DrawDebugSquare(pos.ToVector2(), Color.green, 1000f);
        }
        CellData cellData2 = data[cellData1.position + IntVector2.Up];
        if (cellData2 != null)
        {
          cellData2.occlusionData.cellRoomVisiblityCount = Mathf.RoundToInt(Mathf.Clamp01(1f));
          cellData2.occlusionData.cellRoomVisitedCount = Mathf.RoundToInt(Mathf.Clamp01(1f));
          cellData2.occlusionData.cellVisibleTargetOcclusion = 0.0f;
          cellData2.occlusionData.cellVisitedTargetOcclusion = 0.7f;
          cellData2.occlusionData.overrideOcclusion = true;
          cellData2.occlusionData.cellOcclusion = 0.0f;
        }
        CellData cellData3 = data[cellData1.position + IntVector2.Up * 2];
        if (cellData3 == null)
          return;
        cellData3.occlusionData.cellRoomVisiblityCount = Mathf.RoundToInt(Mathf.Clamp01(1f));
        cellData3.occlusionData.cellRoomVisitedCount = Mathf.RoundToInt(Mathf.Clamp01(1f));
        cellData3.occlusionData.cellVisibleTargetOcclusion = 0.0f;
        cellData3.occlusionData.cellVisitedTargetOcclusion = 0.7f;
        cellData3.occlusionData.overrideOcclusion = true;
        cellData3.occlusionData.cellOcclusion = 0.0f;
      }

      private void HandleTemporaryFailure(PlayerController cp)
      {
        this.m_currentTargetDirectionIndex = 0;
        this.HandleContinuance(cp, this.m_errors != 0 ? 7 : 6);
      }

      private void HandleFailure(PlayerController cp)
      {
        RoomHandler newRoom = (RoomHandler) null;
        foreach (RoomHandler room in GameManager.Instance.Dungeon.data.rooms)
        {
          if (room.area.PrototypeRoomName.Contains("FailRoom"))
          {
            newRoom = room;
            break;
          }
        }
        Vector2 vector2_1 = GameManager.Instance.MainCameraController.transform.position.XY() - cp.transform.position.XY();
        Vector2 vector2_2 = cp.transform.position.XY() - cp.CurrentRoom.area.basePosition.ToVector2();
        Vector2 targetPoint = newRoom.area.basePosition.ToVector2() + vector2_2;
        cp.WarpToPointAndBringCoopPartner(targetPoint, doFollowers: true);
        cp.ForceChangeRoom(newRoom);
        this.HandleNearestExitOcclusion(cp);
        GameManager.Instance.MainCameraController.transform.position = (cp.transform.position.XY() + vector2_1).ToVector3ZUp(GameManager.Instance.MainCameraController.transform.position.z);
      }
    }

}
