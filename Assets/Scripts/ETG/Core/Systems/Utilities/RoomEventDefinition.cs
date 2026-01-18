using System;

#nullable disable

[Serializable]
public class RoomEventDefinition
  {
    public RoomEventTriggerCondition condition;
    public RoomEventTriggerAction action;

    public RoomEventDefinition(RoomEventTriggerCondition c, RoomEventTriggerAction a)
    {
      this.condition = c;
      this.action = a;
    }
  }

