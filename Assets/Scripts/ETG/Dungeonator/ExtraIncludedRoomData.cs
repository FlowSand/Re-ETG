using System;
using UnityEngine;

#nullable disable
namespace Dungeonator
{
  [Serializable]
  public class ExtraIncludedRoomData
  {
    [SerializeField]
    public PrototypeDungeonRoom room;
    [NonSerialized]
    public bool hasBeenProcessed;
  }
}
