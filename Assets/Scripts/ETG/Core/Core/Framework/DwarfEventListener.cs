// Decompiled with JetBrains decompiler
// Type: DwarfEventListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class DwarfEventListener : BraveBehaviour, IEventTriggerable
    {
      public List<DwarfEventListener.Pair> events;
      public Action<int> OnTrigger;

      public void Trigger(int index)
      {
        if (this.OnTrigger != null)
          this.OnTrigger(index);
        for (int index1 = 0; index1 < this.events.Count; ++index1)
        {
          if (this.events[index1].eventIndex == index)
            this.SendPlaymakerEvent(this.events[index1].playmakerEvent);
        }
      }

      protected override void OnDestroy() => base.OnDestroy();

      [Serializable]
      public class Pair
      {
        public int eventIndex;
        public string playmakerEvent;
      }
    }

}
