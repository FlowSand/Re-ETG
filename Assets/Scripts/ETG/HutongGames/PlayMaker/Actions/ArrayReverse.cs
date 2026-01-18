using System.Collections.Generic;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Reverse the order of items in an Array.")]
    [ActionCategory(ActionCategory.Array)]
    public class ArrayReverse : FsmStateAction
    {
        [Tooltip("The Array to reverse.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmArray array;

        public override void Reset() => this.array = (FsmArray) null;

        public override void OnEnter()
        {
            List<object> objectList = new List<object>((IEnumerable<object>) this.array.Values);
            objectList.Reverse();
            this.array.Values = objectList.ToArray();
            this.Finish();
        }
    }
}
