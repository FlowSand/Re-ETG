#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Gets the number of items in an Array.")]
  [ActionCategory(ActionCategory.Array)]
  public class ArrayLength : FsmStateAction
  {
    [Tooltip("The Array Variable.")]
    [UIHint(UIHint.Variable)]
    public FsmArray array;
    [UIHint(UIHint.Variable)]
    [Tooltip("Store the length in an Int Variable.")]
    public FsmInt length;
    [Tooltip("Repeat every frame. Useful if the array is changing and you're waiting for a particular length.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.array = (FsmArray) null;
      this.length = (FsmInt) null;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.length.Value = this.array.Length;
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.length.Value = this.array.Length;
  }
}
