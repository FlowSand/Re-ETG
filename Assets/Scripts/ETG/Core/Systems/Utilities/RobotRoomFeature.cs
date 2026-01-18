// Decompiled with JetBrains decompiler
// Type: RobotRoomFeature
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

  public abstract class RobotRoomFeature
  {
    public IntVector2 LocalBasePosition;
    public IntVector2 LocalDimensions;

    public virtual void Use()
    {
    }

    protected SerializedPath GenerateVerticalPath(IntVector2 BasePosition, IntVector2 Dimensions)
    {
      SerializedPath verticalPath = new SerializedPath(BasePosition + new IntVector2(0, Dimensions.y - 1));
      verticalPath.AddPosition(BasePosition);
      verticalPath.wrapMode = SerializedPath.SerializedPathWrapMode.PingPong;
      SerializedPathNode node1 = verticalPath.nodes[0] with
      {
        placement = SerializedPathNode.SerializedNodePlacement.SouthWest
      };
      verticalPath.nodes[0] = node1;
      SerializedPathNode node2 = verticalPath.nodes[1] with
      {
        placement = SerializedPathNode.SerializedNodePlacement.SouthWest
      };
      verticalPath.nodes[1] = node2;
      return verticalPath;
    }

    protected SerializedPath GenerateHorizontalPath(IntVector2 BasePosition, IntVector2 Dimensions)
    {
      SerializedPath horizontalPath = new SerializedPath(BasePosition + new IntVector2(Dimensions.x - 1, 0));
      horizontalPath.AddPosition(BasePosition);
      horizontalPath.wrapMode = SerializedPath.SerializedPathWrapMode.PingPong;
      SerializedPathNode node1 = horizontalPath.nodes[0] with
      {
        placement = SerializedPathNode.SerializedNodePlacement.SouthWest
      };
      horizontalPath.nodes[0] = node1;
      SerializedPathNode node2 = horizontalPath.nodes[1] with
      {
        placement = SerializedPathNode.SerializedNodePlacement.SouthWest
      };
      horizontalPath.nodes[1] = node2;
      return horizontalPath;
    }

    protected SerializedPath GenerateRectanglePath(IntVector2 BasePosition, IntVector2 Dimensions)
    {
      SerializedPath rectanglePath = new SerializedPath(BasePosition);
      rectanglePath.AddPosition(BasePosition + Dimensions.WithY(0));
      rectanglePath.AddPosition(BasePosition + Dimensions);
      rectanglePath.AddPosition(BasePosition + Dimensions.WithX(0));
      rectanglePath.wrapMode = SerializedPath.SerializedPathWrapMode.Loop;
      SerializedPathNode node1 = rectanglePath.nodes[0] with
      {
        placement = SerializedPathNode.SerializedNodePlacement.SouthWest
      };
      rectanglePath.nodes[0] = node1;
      SerializedPathNode node2 = rectanglePath.nodes[1] with
      {
        placement = SerializedPathNode.SerializedNodePlacement.SouthWest
      };
      rectanglePath.nodes[1] = node2;
      SerializedPathNode node3 = rectanglePath.nodes[2] with
      {
        placement = SerializedPathNode.SerializedNodePlacement.SouthWest
      };
      rectanglePath.nodes[2] = node3;
      SerializedPathNode node4 = rectanglePath.nodes[3] with
      {
        placement = SerializedPathNode.SerializedNodePlacement.SouthWest
      };
      rectanglePath.nodes[3] = node4;
      return rectanglePath;
    }

    protected SerializedPath GenerateRectanglePathInset(
      IntVector2 BasePosition,
      IntVector2 Dimensions)
    {
      BasePosition += new IntVector2(-1, 0);
      Dimensions += IntVector2.One;
      SerializedPath rectanglePathInset = new SerializedPath(BasePosition);
      rectanglePathInset.AddPosition(BasePosition + Dimensions.WithY(0));
      rectanglePathInset.AddPosition(BasePosition + Dimensions);
      rectanglePathInset.AddPosition(BasePosition + Dimensions.WithX(0));
      rectanglePathInset.wrapMode = SerializedPath.SerializedPathWrapMode.Loop;
      SerializedPathNode node1 = rectanglePathInset.nodes[0] with
      {
        placement = SerializedPathNode.SerializedNodePlacement.NorthEast
      };
      rectanglePathInset.nodes[0] = node1;
      SerializedPathNode node2 = rectanglePathInset.nodes[1] with
      {
        placement = SerializedPathNode.SerializedNodePlacement.NorthWest
      };
      rectanglePathInset.nodes[1] = node2;
      SerializedPathNode node3 = rectanglePathInset.nodes[2] with
      {
        placement = SerializedPathNode.SerializedNodePlacement.SouthWest
      };
      rectanglePathInset.nodes[2] = node3;
      SerializedPathNode node4 = rectanglePathInset.nodes[3] with
      {
        placement = SerializedPathNode.SerializedNodePlacement.SouthEast
      };
      rectanglePathInset.nodes[3] = node4;
      return rectanglePathInset;
    }

    public abstract bool AcceptableInIdea(
      RobotDaveIdea idea,
      IntVector2 dim,
      bool isInternal,
      int numFeatures);

    public virtual bool CanContainOtherFeature() => false;

    public virtual int RequiredInsetForOtherFeature() => 0;

    protected PrototypePlacedObjectData PlaceObject(
      DungeonPlaceable item,
      PrototypeDungeonRoom room,
      IntVector2 position,
      int targetObjectLayer)
    {
      if (room.CheckRegionOccupied(position.x, position.y, item.GetWidth(), item.GetHeight()))
        return (PrototypePlacedObjectData) null;
      Vector2 vector2 = position.ToVector2();
      PrototypePlacedObjectData placedObjectData = new PrototypePlacedObjectData();
      placedObjectData.fieldData = new List<PrototypePlacedObjectFieldData>();
      placedObjectData.instancePrerequisites = new DungeonPrerequisite[0];
      placedObjectData.placeableContents = item;
      placedObjectData.contentsBasePosition = vector2;
      int count = room.placedObjects.Count;
      room.placedObjects.Add(placedObjectData);
      room.placedObjectPositions.Add(vector2);
      for (int index1 = 0; index1 < item.GetWidth(); ++index1)
      {
        for (int index2 = 0; index2 < item.GetHeight(); ++index2)
          room.ForceGetCellDataAtPoint(position.x + index1, position.y + index2).placedObjectRUBELIndex = count;
      }
      return placedObjectData;
    }

    protected PrototypePlacedObjectData PlaceObject(
      DungeonPlaceableBehaviour item,
      PrototypeDungeonRoom room,
      IntVector2 position,
      int targetObjectLayer)
    {
      if (room.CheckRegionOccupied(position.x, position.y, item.GetWidth(), item.GetHeight()))
        return (PrototypePlacedObjectData) null;
      Vector2 vector2 = position.ToVector2();
      PrototypePlacedObjectData placedObjectData = new PrototypePlacedObjectData();
      placedObjectData.fieldData = new List<PrototypePlacedObjectFieldData>();
      placedObjectData.instancePrerequisites = new DungeonPrerequisite[0];
      placedObjectData.nonenemyBehaviour = item;
      placedObjectData.contentsBasePosition = vector2;
      if (targetObjectLayer == -1)
      {
        int count = room.placedObjects.Count;
        room.placedObjects.Add(placedObjectData);
        room.placedObjectPositions.Add(vector2);
        for (int index1 = 0; index1 < item.GetWidth(); ++index1)
        {
          for (int index2 = 0; index2 < item.GetHeight(); ++index2)
            room.ForceGetCellDataAtPoint(position.x + index1, position.y + index2).placedObjectRUBELIndex = count;
        }
      }
      else
      {
        PrototypeRoomObjectLayer additionalObjectLayer = room.additionalObjectLayers[targetObjectLayer];
        int count = additionalObjectLayer.placedObjects.Count;
        additionalObjectLayer.placedObjects.Add(placedObjectData);
        additionalObjectLayer.placedObjectBasePositions.Add(vector2);
        for (int index3 = 0; index3 < item.GetWidth(); ++index3)
        {
          for (int index4 = 0; index4 < item.GetHeight(); ++index4)
            room.ForceGetCellDataAtPoint(position.x + index3, position.y + index4).additionalPlacedObjectIndices[targetObjectLayer] = count;
        }
      }
      return placedObjectData;
    }

    public abstract void Develop(
      PrototypeDungeonRoom room,
      RobotDaveIdea idea,
      int targetObjectLayer);
  }

