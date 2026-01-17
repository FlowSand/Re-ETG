// Decompiled with JetBrains decompiler
// Type: RoomEventDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
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

}
