// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.ArrayResize
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace HutongGames.PlayMaker.Actions;

[ActionCategory(ActionCategory.Array)]
[Tooltip("Resize an array.")]
public class ArrayResize : FsmStateAction
{
  [Tooltip("The Array Variable to resize")]
  [UIHint(UIHint.Variable)]
  [RequiredField]
  public FsmArray array;
  [Tooltip("The new size of the array.")]
  public FsmInt newSize;
  [Tooltip("The event to trigger if the new size is out of range")]
  public FsmEvent sizeOutOfRangeEvent;

  public override void OnEnter()
  {
    if (this.newSize.Value >= 0)
    {
      this.array.Resize(this.newSize.Value);
    }
    else
    {
      this.LogError("Size out of range: " + (object) this.newSize.Value);
      this.Fsm.Event(this.sizeOutOfRangeEvent);
    }
    this.Finish();
  }
}
