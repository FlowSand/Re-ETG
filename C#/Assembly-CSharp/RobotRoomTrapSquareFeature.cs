// Decompiled with JetBrains decompiler
// Type: RobotRoomTrapSquareFeature
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class RobotRoomTrapSquareFeature : RobotRoomFeature
{
  public override bool CanContainOtherFeature() => true;

  public override int RequiredInsetForOtherFeature() => 4;

  public override bool AcceptableInIdea(
    RobotDaveIdea idea,
    IntVector2 dim,
    bool isInternal,
    int numFeatures)
  {
    return (idea.UseFloorFlameTraps || idea.UseFloorPitTraps || idea.UseFloorSpikeTraps) && dim.x > 6 && dim.y > 6;
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
    int num1 = intList[Random.Range(0, intList.Count)];
    DungeonPlaceableBehaviour placeableBehaviour = (DungeonPlaceableBehaviour) null;
    switch (num1)
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
    int num2 = this.LocalBasePosition.x + 2;
    int num3 = this.LocalBasePosition.y + 2;
    int num4 = this.LocalBasePosition.x + this.LocalDimensions.x - 2;
    int num5 = this.LocalBasePosition.y + this.LocalDimensions.y - 2;
    if ((num4 - num2) % width != 0)
      --num4;
    if ((num5 - num3) % width != 0)
      --num5;
    for (int x = num2; x < num4; x += width)
    {
      for (int y = num3; y < num5; y += width)
      {
        if (x == num2 || x == num4 - width || y == num3 || y == num5 - width)
        {
          IntVector2 position = new IntVector2(x, y);
          this.PlaceObject(placeableBehaviour, room, position, targetObjectLayer);
        }
      }
    }
  }
}
