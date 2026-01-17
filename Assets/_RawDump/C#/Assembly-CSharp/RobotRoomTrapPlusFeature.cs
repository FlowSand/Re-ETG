// Decompiled with JetBrains decompiler
// Type: RobotRoomTrapPlusFeature
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class RobotRoomTrapPlusFeature : RobotRoomFeature
{
  public override bool AcceptableInIdea(
    RobotDaveIdea idea,
    IntVector2 dim,
    bool isInternal,
    int numFeatures)
  {
    return (idea.UseFloorFlameTraps || idea.UseFloorPitTraps || idea.UseFloorSpikeTraps) && numFeatures == 1;
  }

  public override void Develop(
    PrototypeDungeonRoom room,
    RobotDaveIdea idea,
    int targetObjectLayer)
  {
    List<int> intList = new List<int>();
    if (idea.UseFloorPitTraps)
      intList.Add(0);
    if (idea.UseFloorSpikeTraps)
      intList.Add(1);
    if (idea.UseFloorFlameTraps)
      intList.Add(2);
    int num = intList[Random.Range(0, intList.Count)];
    DungeonPlaceableBehaviour placeableBehaviour = (DungeonPlaceableBehaviour) null;
    switch (num)
    {
      case 0:
        placeableBehaviour = RobotDave.GetPitTrap();
        break;
      case 1:
        placeableBehaviour = RobotDave.GetSpikesTrap();
        break;
      case 2:
        placeableBehaviour = RobotDave.GetFloorFlameTrap();
        break;
    }
    int width = placeableBehaviour.GetWidth();
    if (this.LocalDimensions.x % width == 0)
    {
      int y = this.LocalBasePosition.y + Mathf.FloorToInt((float) this.LocalDimensions.y / 2f) - (width - 1);
      for (int index = 0; index < this.LocalDimensions.x; index += width)
        this.PlaceObject(placeableBehaviour, room, new IntVector2(this.LocalBasePosition.x + index, y), targetObjectLayer);
    }
    if (this.LocalDimensions.y % width != 0)
      return;
    int x = this.LocalBasePosition.x + Mathf.FloorToInt((float) this.LocalDimensions.x / 2f) - (width - 1);
    for (int index = 0; index < this.LocalDimensions.y; index += width)
      this.PlaceObject(placeableBehaviour, room, new IntVector2(x, this.LocalBasePosition.y + index), targetObjectLayer);
  }
}
