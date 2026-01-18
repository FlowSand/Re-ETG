using Dungeonator;
using UnityEngine;

#nullable disable

public class SecretRoomExitData
  {
    public GameObject colliderObject;
    public DungeonData.Direction exitDirection;

    public SecretRoomExitData(GameObject g, DungeonData.Direction d)
    {
      this.colliderObject = g;
      this.exitDirection = d;
    }
  }

