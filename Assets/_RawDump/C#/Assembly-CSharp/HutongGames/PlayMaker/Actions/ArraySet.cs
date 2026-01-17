// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ArraySet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Array)]
[Tooltip("Set the value at an index. Index must be between 0 and the number of items -1. First item is index 0.")]
public class ArraySet : FsmStateAction
{
  [UIHint(UIHint.Variable)]
  [Tooltip("The Array Variable to use.")]
  [RequiredField]
  public FsmArray array;
  [Tooltip("The index into the array.")]
  public FsmInt index;
  [MatchElementType("array")]
  [RequiredField]
  [Tooltip("Set the value of the array at the specified index.")]
  public FsmVar value;
  [Tooltip("Repeat every frame while the state is active.")]
  public bool everyFrame;
  [ActionSection("Events")]
  [Tooltip("The event to trigger if the index is out of range")]
  public FsmEvent indexOutOfRange;

  public override void Reset()
  {
    this.array = (FsmArray) null;
    this.index = (FsmInt) null;
    this.value = (FsmVar) null;
    this.everyFrame = false;
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
    if (this.array.IsNone)
      return;
    if (this.index.Value >= 0 && this.index.Value < this.array.Length)
    {
      this.value.UpdateValue();
      this.array.Set(this.index.Value, this.value.GetValue());
    }
    else
      this.Fsm.Event(this.indexOutOfRange);
  }
}
