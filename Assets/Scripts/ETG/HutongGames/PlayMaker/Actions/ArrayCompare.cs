using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Tests if 2 Array Variables have the same values.")]
    public class ArrayCompare : FsmStateAction
    {
        [Tooltip("The first Array Variable to test.")]
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmArray array1;
        [Tooltip("The second Array Variable to test.")]
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmArray array2;
        [Tooltip("Event to send if the 2 arrays have the same values.")]
        public FsmEvent SequenceEqual;
        [Tooltip("Event to send if the 2 arrays have different values.")]
        public FsmEvent SequenceNotEqual;
        [Tooltip("Store the result in a Bool variable.")]
        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;
        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.array1 = (FsmArray) null;
            this.array2 = (FsmArray) null;
            this.SequenceEqual = (FsmEvent) null;
            this.SequenceNotEqual = (FsmEvent) null;
        }

        public override void OnEnter()
        {
            this.DoSequenceEqual();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        private void DoSequenceEqual()
        {
            if (this.array1.Values == null || this.array2.Values == null)
                return;
            this.storeResult.Value = ((IEnumerable<object>) this.array1.Values).SequenceEqual<object>((IEnumerable<object>) this.array2.Values);
            this.Fsm.Event(!this.storeResult.Value ? this.SequenceNotEqual : this.SequenceEqual);
        }
    }
}
