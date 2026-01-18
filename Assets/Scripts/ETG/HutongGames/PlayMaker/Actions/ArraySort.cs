using System.Collections.Generic;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [Tooltip("Sort items in an Array.")]
  [ActionCategory(ActionCategory.Array)]
  public class ArraySort : FsmStateAction
  {
    [Tooltip("The Array to sort.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmArray array;

    public override void Reset() => this.array = (FsmArray) null;

    public override void OnEnter()
    {
      List<object> objectList = new List<object>((IEnumerable<object>) this.array.Values);
      objectList.Sort();
      this.array.Values = objectList.ToArray();
      this.Finish();
    }
  }
}
