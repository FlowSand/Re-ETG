// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ArrayGet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Array)]
  [Tooltip("Get a value at an index. Index must be between 0 and the number of items -1. First item is index 0.")]
  public class ArrayGet : FsmStateAction
  {
    [Tooltip("The Array Variable to use.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmArray array;
    [Tooltip("The index into the array.")]
    public FsmInt index;
    [UIHint(UIHint.Variable)]
    [Tooltip("Store the value in a variable.")]
    [RequiredField]
    [MatchElementType("array")]
    public FsmVar storeValue;
    [Tooltip("Repeat every frame while the state is active.")]
    public bool everyFrame;
    [Tooltip("The event to trigger if the index is out of range")]
    [ActionSection("Events")]
    public FsmEvent indexOutOfRange;

    public override void Reset()
    {
      this.array = (FsmArray) null;
      this.index = (FsmInt) null;
      this.everyFrame = false;
      this.storeValue = (FsmVar) null;
      this.indexOutOfRange = (FsmEvent) null;
    }

    public override void OnEnter()
    {
      this.DoGetValue();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoGetValue();

    private void DoGetValue()
    {
      if (this.array.IsNone || this.storeValue.IsNone)
        return;
      if (this.index.Value >= 0 && this.index.Value < this.array.Length)
        this.storeValue.SetValue(this.array.Get(this.index.Value));
      else
        this.Fsm.Event(this.indexOutOfRange);
    }
  }
}
