// Decompiled with JetBrains decompiler
// Type: Dungeonator.FlowNodeBuildData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace Dungeonator
{
  public class FlowNodeBuildData
  {
    public DungeonFlowNode node;
    public bool usesOverrideCategory;
    public PrototypeDungeonRoom.RoomCategory overrideCategory;
    public RoomHandler room;
    public bool unbuilt = true;
    public PrototypeRoomExit sourceExit;
    public PrototypeRoomExit roomEntrance;
    public RoomHandler sourceRoom;
    public List<FlowNodeBuildData> childBuildData;

    public FlowNodeBuildData(DungeonFlowNode n) => this.node = n;

    public void MarkExits()
    {
      this.room.area.instanceUsedExits.Add(this.roomEntrance);
      this.sourceRoom.area.instanceUsedExits.Add(this.sourceExit);
      this.room.parentRoom = this.sourceRoom;
      this.room.connectedRooms.Add(this.sourceRoom);
      this.room.connectedRoomsByExit.Add(this.roomEntrance, this.sourceRoom);
      this.sourceRoom.childRooms.Add(this.room);
      this.sourceRoom.connectedRooms.Add(this.room);
      this.sourceRoom.connectedRoomsByExit.Add(this.sourceExit, this.room);
    }

    public void UnmarkExits()
    {
      this.room.area.instanceUsedExits.Remove(this.roomEntrance);
      this.sourceRoom.area.instanceUsedExits.Remove(this.sourceExit);
      this.room.parentRoom = (RoomHandler) null;
      this.room.connectedRooms.Remove(this.sourceRoom);
      this.room.connectedRoomsByExit.Remove(this.roomEntrance);
      this.sourceRoom.childRooms.Remove(this.room);
      this.sourceRoom.connectedRooms.Remove(this.room);
      this.sourceRoom.connectedRoomsByExit.Remove(this.sourceExit);
    }
  }
}
