// Decompiled with JetBrains decompiler
// Type: RuntimeRoomExitData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
