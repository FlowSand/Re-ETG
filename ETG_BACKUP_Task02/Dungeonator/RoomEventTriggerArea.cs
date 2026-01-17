// Decompiled with JetBrains decompiler
// Type: Dungeonator.RoomEventTriggerArea
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Dungeonator;

public class RoomEventTriggerArea
{
  public HashSet<IntVector2> triggerCells;
  public IntVector2 initialPosition;
  public List<IEventTriggerable> events;
  [NonSerialized]
  public GameObject tempDataObject;

  public RoomEventTriggerArea()
  {
    this.triggerCells = new HashSet<IntVector2>();
    this.events = new List<IEventTriggerable>();
  }

  public RoomEventTriggerArea(PrototypeEventTriggerArea prototype, IntVector2 basePosition)
  {
    this.triggerCells = new HashSet<IntVector2>();
    this.events = new List<IEventTriggerable>();
    for (int index = 0; index < prototype.triggerCells.Count; ++index)
    {
      IntVector2 key = prototype.triggerCells[index].ToIntVector2() + basePosition;
      GameManager.Instance.Dungeon.data[key].cellVisualData.containsObjectSpaceStamp = true;
      this.triggerCells.Add(key);
      if (index == 0)
        this.initialPosition = key;
    }
  }

  public void Trigger(int eventIndex)
  {
    for (int index = 0; index < this.events.Count; ++index)
      this.events[index].Trigger(eventIndex);
  }

  public void AddGameObject(GameObject g)
  {
    if (!(g.GetComponentInChildren(typeof (IEventTriggerable)) is IEventTriggerable componentInChildren))
      return;
    this.events.Add(componentInChildren);
    if (!(componentInChildren is HangingObjectController))
      return;
    for (int x = 0; x < 2; ++x)
    {
      for (int y = 0; y < 3; ++y)
      {
        IntVector2 key = this.initialPosition + new IntVector2(x, y);
        GameManager.Instance.Dungeon.data[key].cellVisualData.containsWallSpaceStamp = true;
        GameManager.Instance.Dungeon.data[key].cellVisualData.containsObjectSpaceStamp = true;
      }
    }
  }
}
