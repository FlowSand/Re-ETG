// Decompiled with JetBrains decompiler
// Type: Dungeonator.SemioticDungeonGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.IO;
using UnityEngine;

#nullable disable
namespace Dungeonator;

public class SemioticDungeonGenerator : IDungeonGenerator
{
  public bool RAPID_DEBUG_ITERATION_MODE;
  public int RAPID_DEBUG_ITERATION_INDEX;
  private SemioticDungeonGenSettings m_patternSettings;
  private int m_numberRoomCells;

  public SemioticDungeonGenerator(Dungeon d, SemioticDungeonGenSettings sdgs)
  {
    this.m_patternSettings = sdgs;
  }

  public DungeonData GenerateDungeonLayout()
  {
    PrototypeDungeonRoom prototypeDungeonRoom = this.GetRandomEntranceRoomFromList(this.m_patternSettings.flows[0].fallbackRoomTable.GetCompiledList());
    if ((Object) prototypeDungeonRoom == (Object) null)
      prototypeDungeonRoom = this.m_patternSettings.flows[0].fallbackRoomTable.SelectByWeight().room;
    SemioticLayoutManager layout = (SemioticLayoutManager) null;
    DungeonFlowBuilder dungeonFlowBuilder = (DungeonFlowBuilder) null;
    CellArea cellArea = (CellArea) null;
    bool flag1 = true;
    int num1 = 0;
    int num2 = 10;
    bool flag2;
    do
    {
      do
      {
        BraveMemory.DoCollect();
        if (num1 == num2)
        {
          Debug.LogError((object) "DUNGEON GENERATION FAILED.");
          flag1 = false;
          goto label_12;
        }
        ++num1;
        cellArea = new CellArea(IntVector2.Zero, new IntVector2(prototypeDungeonRoom.Width, prototypeDungeonRoom.Height));
        cellArea.prototypeRoom = prototypeDungeonRoom;
        cellArea.instanceUsedExits = new List<PrototypeRoomExit>();
        RoomHandler roomHandler = new RoomHandler(cellArea);
        roomHandler.distanceFromEntrance = 0;
        layout = new SemioticLayoutManager();
        layout.StampCellAreaToLayout(roomHandler);
        dungeonFlowBuilder = new DungeonFlowBuilder(this.m_patternSettings.flows[0], layout);
        flag2 = dungeonFlowBuilder.Build(roomHandler);
      }
      while (!flag2);
      for (int index = 0; index < this.m_patternSettings.mandatoryExtraRooms.Count; ++index)
      {
        flag2 = dungeonFlowBuilder.AttemptAppendExtraRoom(this.m_patternSettings.mandatoryExtraRooms[index]);
        if (!flag2)
          break;
      }
    }
    while (!flag2);
    Debug.Log((object) ("DUNGEON GENERATION SUCCEEDED ON ATTEMPT #" + (object) num1));
label_12:
    if (flag1)
      dungeonFlowBuilder.AppendCapChains();
    IntVector2 minimumCellPosition = layout.GetMinimumCellPosition();
    IntVector2 maximumCellPosition = layout.GetMaximumCellPosition();
    IntVector2 offset = new IntVector2(-minimumCellPosition.x + 10, -minimumCellPosition.y + 10);
    IntVector2 intVector2 = maximumCellPosition - minimumCellPosition;
    layout.HandleOffsetRooms(offset);
    dungeonFlowBuilder.DebugActionLines();
    if (this.RAPID_DEBUG_ITERATION_MODE)
    {
      Texture2D tex = new Texture2D(intVector2.x + 20, intVector2.y + 20);
      Color[] colors = new Color[(intVector2.x + 20) * (intVector2.y + 20)];
      for (int index1 = 0; index1 < intVector2.x + 20; ++index1)
      {
        for (int index2 = 0; index2 < intVector2.y + 20; ++index2)
          colors[index2 * (intVector2.x + 20) + index1] = !flag1 ? new Color(0.5f, 0.0f, 0.0f) : new Color(0.0f, 0.5f, 0.0f);
      }
      tex.SetPixels(colors);
      tex.Apply();
      byte[] png = tex.EncodeToPNG();
      using (BinaryWriter binaryWriter = new BinaryWriter((Stream) File.Open($"{Application.dataPath}/DungeonDebug/debug_{this.RAPID_DEBUG_ITERATION_INDEX.ToString()}.png", FileMode.Create)))
        binaryWriter.Write(png);
      return (DungeonData) null;
    }
    CellData[][] data = new CellData[intVector2.x + 20][];
    for (int x = 0; x < data.Length; ++x)
    {
      data[x] = new CellData[intVector2.y + 20];
      for (int y = 0; y < data[x].Length; ++y)
        data[x][y] = new CellData(x, y);
    }
    DungeonData dungeonLayout = new DungeonData(data);
    List<RoomHandler> rooms = layout.Rooms;
    dungeonLayout.InitializeCoreData(rooms);
    dungeonLayout.Entrance = this.GetRoomHandlerByArea(rooms, cellArea);
    dungeonLayout.Exit = dungeonFlowBuilder.EndRoom;
    return dungeonLayout;
  }

  private PrototypeDungeonRoom GetRandomEntranceRoomFromList(List<WeightedRoom> source)
  {
    List<PrototypeDungeonRoom> prototypeDungeonRoomList = new List<PrototypeDungeonRoom>();
    for (int index = 0; index < source.Count; ++index)
    {
      if (source[index].room.category == PrototypeDungeonRoom.RoomCategory.ENTRANCE)
        prototypeDungeonRoomList.Add(source[index].room);
    }
    return prototypeDungeonRoomList.Count > 0 ? prototypeDungeonRoomList[BraveRandom.GenerationRandomRange(0, prototypeDungeonRoomList.Count)] : (PrototypeDungeonRoom) null;
  }

  private RoomHandler GetRoomHandlerByArea(List<RoomHandler> rooms, CellArea area)
  {
    for (int index = 0; index < rooms.Count; ++index)
    {
      if (rooms[index].area == area)
        return rooms[index];
    }
    return (RoomHandler) null;
  }

  private void DrawDebugSquare(IntVector2 pos, Color col)
  {
    Debug.DrawLine((Vector3) pos.ToVector2(), (Vector3) (pos.ToVector2() + Vector2.up), col, 1000f);
    Debug.DrawLine((Vector3) pos.ToVector2(), (Vector3) (pos.ToVector2() + Vector2.right), col, 1000f);
    Debug.DrawLine((Vector3) (pos.ToVector2() + Vector2.up), (Vector3) (pos.ToVector2() + Vector2.right + Vector2.up), col, 1000f);
    Debug.DrawLine((Vector3) (pos.ToVector2() + Vector2.right), (Vector3) (pos.ToVector2() + Vector2.right + Vector2.up), col, 1000f);
  }
}
