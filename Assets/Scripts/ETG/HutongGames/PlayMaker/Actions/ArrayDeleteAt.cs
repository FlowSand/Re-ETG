// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ArrayDeleteAt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Delete the item at an index. Index must be between 0 and the number of items -1. First item is index 0.")]
  [ActionCategory(ActionCategory.Array)]
  public class ArrayDeleteAt : FsmStateAction
  {
    [Tooltip("The Array Variable to use.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmArray array;
    [Tooltip("The index into the array.")]
    public FsmInt index;
    [Tooltip("The event to trigger if the index is out of range")]
    [ActionSection("Result")]
    public FsmEvent indexOutOfRangeEvent;

    public override void Reset()
    {
      this.array = (FsmArray) null;
      this.index = (FsmInt) null;
      this.indexOutOfRangeEvent = (FsmEvent) null;
    }

    public override void OnEnter()
    {
      this.DoDeleteAt();
      this.Finish();
    }

    private void DoDeleteAt()
    {
      if (this.index.Value >= 0 && this.index.Value < this.array.Length)
      {
        List<object> objectList = new List<object>((IEnumerable<object>) this.array.Values);
        objectList.RemoveAt(this.index.Value);
        this.array.Values = objectList.ToArray();
      }
      else
        this.Fsm.Event(this.indexOutOfRangeEvent);
    }
  }
}
