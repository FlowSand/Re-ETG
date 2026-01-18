using System;

#nullable disable

public class RuntimeRoomExitData
  {
    [NonSerialized]
    public PrototypeRoomExit referencedExit;
    [NonSerialized]
    public int additionalExitLength;
    [NonSerialized]
    public bool jointedExit;
    [NonSerialized]
    public RuntimeRoomExitData linkedExit;
    [NonSerialized]
    public bool oneWayDoor;
    [NonSerialized]
    public bool isLockedDoor;
    [NonSerialized]
    public bool isCriticalPath;
    [NonSerialized]
    public bool isWarpWingStart;
    [NonSerialized]
    public DungeonFlowNode.ForcedDoorType forcedDoorType;
    [NonSerialized]
    public WarpWingPortalController warpWingPortal;

    public RuntimeRoomExitData(PrototypeRoomExit exit) => this.referencedExit = exit;

    public int TotalExitLength => this.additionalExitLength + this.referencedExit.exitLength;

    public IntVector2 HalfExitAttachPoint
    {
      get => this.referencedExit.GetHalfExitAttachPoint(this.TotalExitLength);
    }

    public IntVector2 ExitOrigin => this.referencedExit.GetExitOrigin(this.TotalExitLength);
  }

