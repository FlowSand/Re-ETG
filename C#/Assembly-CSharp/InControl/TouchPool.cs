// Decompiled with JetBrains decompiler
// Type: InControl.TouchPool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

#nullable disable
namespace InControl;

public class TouchPool
{
  public readonly ReadOnlyCollection<Touch> Touches;
  private List<Touch> usedTouches;
  private List<Touch> freeTouches;

  public TouchPool(int capacity)
  {
    this.freeTouches = new List<Touch>(capacity);
    for (int index = 0; index < capacity; ++index)
      this.freeTouches.Add(new Touch());
    this.usedTouches = new List<Touch>(capacity);
    this.Touches = new ReadOnlyCollection<Touch>((IList<Touch>) this.usedTouches);
  }

  public TouchPool()
    : this(16 /*0x10*/)
  {
  }

  public Touch FindOrCreateTouch(int fingerId)
  {
    int count = this.usedTouches.Count;
    for (int index = 0; index < count; ++index)
    {
      Touch usedTouch = this.usedTouches[index];
      if (usedTouch.fingerId == fingerId)
        return usedTouch;
    }
    Touch orCreateTouch = this.NewTouch();
    orCreateTouch.fingerId = fingerId;
    this.usedTouches.Add(orCreateTouch);
    return orCreateTouch;
  }

  public Touch FindTouch(int fingerId)
  {
    int count = this.usedTouches.Count;
    for (int index = 0; index < count; ++index)
    {
      Touch usedTouch = this.usedTouches[index];
      if (usedTouch.fingerId == fingerId)
        return usedTouch;
    }
    return (Touch) null;
  }

  private Touch NewTouch()
  {
    int count = this.freeTouches.Count;
    if (count <= 0)
      return new Touch();
    Touch freeTouch = this.freeTouches[count - 1];
    this.freeTouches.RemoveAt(count - 1);
    return freeTouch;
  }

  public void FreeTouch(Touch touch)
  {
    touch.Reset();
    this.freeTouches.Add(touch);
  }

  public void FreeEndedTouches()
  {
    for (int index = this.usedTouches.Count - 1; index >= 0; --index)
    {
      if (this.usedTouches[index].phase == TouchPhase.Ended)
        this.usedTouches.RemoveAt(index);
    }
  }
}
