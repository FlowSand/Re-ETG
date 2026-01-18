using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Performs math operations on 2 Floats: Add, Subtract, Multiply, Divide, Min, Max.")]
    [ActionCategory(ActionCategory.Math)]
    public class FloatOperator : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The first float.")]
        [RequiredField]
        public FsmFloat float1;
        [HutongGames.PlayMaker.Tooltip("The second float.")]
        [RequiredField]
        public FsmFloat float2;
        [HutongGames.PlayMaker.Tooltip("The math operation to perform on the floats.")]
        public FloatOperator.Operation operation;
        [HutongGames.PlayMaker.Tooltip("Store the result of the operation in a float variable.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmFloat storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the variables are changing.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.float1 = (FsmFloat) null;
            this.float2 = (FsmFloat) null;
            this.operation = FloatOperator.Operation.Add;
            this.storeResult = (FsmFloat) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoFloatOperator();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoFloatOperator();

        private void DoFloatOperator()
        {
            float a = this.float1.Value;
            float b = this.float2.Value;
            switch (this.operation)
            {
                case FloatOperator.Operation.Add:
                    this.storeResult.Value = a + b;
                    break;
                case FloatOperator.Operation.Subtract:
                    this.storeResult.Value = a - b;
                    break;
                case FloatOperator.Operation.Multiply:
                    this.storeResult.Value = a * b;
                    break;
                case FloatOperator.Operation.Divide:
                    this.storeResult.Value = a / b;
                    break;
                case FloatOperator.Operation.Min:
                    this.storeResult.Value = Mathf.Min(a, b);
                    break;
                case FloatOperator.Operation.Max:
                    this.storeResult.Value = Mathf.Max(a, b);
                    break;
            }
        }

        public enum Operation
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Min,
            Max,
        }
    }
}
