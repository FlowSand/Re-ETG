using Dungeonator;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class SenseOfDirectionItem : PlayerItem
  {
    public GameObject arrowVFX;

    protected override void DoEffect(PlayerController user)
    {
      RoomHandler target = (RoomHandler) null;
      for (int index = 0; index < GameManager.Instance.Dungeon.data.rooms.Count; ++index)
      {
        RoomHandler room = GameManager.Instance.Dungeon.data.rooms[index];
        if (room.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.EXIT)
        {
          target = room;
          break;
        }
      }
      if (target == null)
      {
        List<AIActor> allEnemies = StaticReferenceManager.AllEnemies;
        for (int index = 0; index < allEnemies.Count; ++index)
        {
          if ((bool) (Object) allEnemies[index] && (bool) (Object) allEnemies[index].GetComponent<LichDeathController>())
          {
            target = allEnemies[index].ParentRoom;
            break;
          }
        }
      }
      if (target == null)
      {
        Debug.LogError((object) "Using SenseOfDirection in Dungeon with no EXIT?");
      }
      else
      {
        RoomHandler currentRoom = user.CurrentRoom;
        if (target == currentRoom)
          return;
        List<RoomHandler> pathBetweenNodes = SenseOfDirectionItem.FindPathBetweenNodes(currentRoom, target, GameManager.Instance.Dungeon.data.rooms);
        IntVector2 intVector2 = user.CenterPosition.ToIntVector2(VectorConversions.Floor);
        if (pathBetweenNodes != null && pathBetweenNodes.Count > 0 && (pathBetweenNodes[0] != currentRoom || pathBetweenNodes.Count >= 2))
        {
          RoomHandler otherRoom = pathBetweenNodes[0] != currentRoom ? pathBetweenNodes[0] : pathBetweenNodes[1];
          intVector2 = currentRoom.GetExitDefinitionForConnectedRoom(otherRoom).GetUpstreamBasePosition();
        }
        Vector2 vector2 = intVector2.ToCenterVector2() - user.CenterPosition;
        SpawnManager.SpawnVFX(this.arrowVFX, user.SpriteBottomCenter + vector2.ToVector3ZUp().normalized, Quaternion.Euler(0.0f, 0.0f, BraveMathCollege.Atan2Degrees(vector2)), false);
      }
    }

    protected override void OnDestroy() => base.OnDestroy();

    public static List<RoomHandler> FindPathBetweenNodes(
      RoomHandler origin,
      RoomHandler target,
      List<RoomHandler> allRooms)
    {
      Dictionary<RoomHandler, int> dictionary1 = new Dictionary<RoomHandler, int>();
      Dictionary<RoomHandler, RoomHandler> dictionary2 = new Dictionary<RoomHandler, RoomHandler>();
      for (int index = 0; index < allRooms.Count; ++index)
      {
        int num = int.MaxValue;
        if (allRooms[index] == origin)
          num = 0;
        if (!dictionary1.ContainsKey(allRooms[index]))
          dictionary1.Add(allRooms[index], num);
      }
      RoomHandler key1 = origin;
      int num1 = 1;
      do
      {
        List<RoomHandler> connectedRooms = key1.connectedRooms;
        for (int index = 0; index < connectedRooms.Count; ++index)
        {
          if (connectedRooms[index] == target)
          {
            if (dictionary2.ContainsKey(connectedRooms[index]))
            {
              dictionary2[connectedRooms[index]] = key1;
              goto label_28;
            }
            dictionary2.Add(connectedRooms[index], key1);
            goto label_28;
          }
          if (dictionary1.ContainsKey(connectedRooms[index]) && dictionary1[connectedRooms[index]] > num1)
          {
            dictionary1[connectedRooms[index]] = num1;
            if (dictionary2.ContainsKey(connectedRooms[index]))
              dictionary2[connectedRooms[index]] = key1;
            else
              dictionary2.Add(connectedRooms[index], key1);
          }
        }
        dictionary1.Remove(key1);
        if (dictionary1.Count == 0)
          return (List<RoomHandler>) null;
        key1 = (RoomHandler) null;
        num1 = int.MaxValue;
        foreach (RoomHandler key2 in dictionary1.Keys)
        {
          if (dictionary1[key2] < num1)
          {
            key1 = key2;
            num1 = dictionary1[key2];
          }
        }
      }
      while (key1 != null);
  label_28:
      if (!dictionary2.ContainsKey(target))
        return (List<RoomHandler>) null;
      List<RoomHandler> pathBetweenNodes = new List<RoomHandler>();
      for (RoomHandler key3 = target; key3 != null; key3 = !dictionary2.ContainsKey(key3) ? (RoomHandler) null : dictionary2[key3])
        pathBetweenNodes.Insert(0, key3);
      return pathBetweenNodes;
    }
  }

