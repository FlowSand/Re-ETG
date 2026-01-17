// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ArrayContains
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Check if an Array contains a value. Optionally get its index.")]
  [ActionCategory(ActionCategory.Array)]
  public class ArrayContains : FsmStateAction
  {
    [Tooltip("The Array Variable to use.")]
    [RequiredField]
    [UIHint(UIHint.Variable)]
    public FsmArray array;
    [Tooltip("The value to check against in the array.")]
    [RequiredField]
    [MatchElementType("array")]
    public FsmVar value;
    [UIHint(UIHint.Variable)]
    [Tooltip("The index of the value in the array.")]
    [ActionSection("Result")]
    public FsmInt index;
    [UIHint(UIHint.Variable)]
    [Tooltip("Store in a bool whether it contains that element or not (described below)")]
    public FsmBool isContained;
    [Tooltip("Event sent if the array contains that element (described below)")]
    public FsmEvent isContainedEvent;
    [Tooltip("Event sent if the array does not contains that element (described below)")]
    public FsmEvent isNotContainedEvent;

    public override void Reset()
    {
      this.array = (FsmArray) null;
      this.value = (FsmVar) null;
      this.index = (FsmInt) null;
      this.isContained = (FsmBool) null;
      this.isContainedEvent = (FsmEvent) null;
      this.isNotContainedEvent = (FsmEvent) null;
    }

    public override void OnEnter()
    {
      this.DoCheckContainsValue();
      this.Finish();
    }

    private void DoCheckContainsValue()
    {
      this.value.UpdateValue();
      int num = Array.IndexOf<object>(this.array.Values, this.value.GetValue());
      bool flag = num != -1;
      this.isContained.Value = flag;
      this.index.Value = num;
      if (flag)
        this.Fsm.Event(this.isContainedEvent);
      else
        this.Fsm.Event(this.isNotContainedEvent);
    }
  }
}
