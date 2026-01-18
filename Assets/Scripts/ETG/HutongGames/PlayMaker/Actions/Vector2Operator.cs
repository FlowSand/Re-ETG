using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Performs most possible operations on 2 Vector2: Dot product, Distance, Angle, Add, Subtract, Multiply, Divide, Min, Max")]
    [ActionCategory(ActionCategory.Vector2)]
    public class Vector2Operator : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The first vector")]
        [RequiredField]
        public FsmVector2 vector1;
        [HutongGames.PlayMaker.Tooltip("The second vector")]
        [RequiredField]
        public FsmVector2 vector2;
        [HutongGames.PlayMaker.Tooltip("The operation")]
        public Vector2Operator.Vector2Operation operation = Vector2Operator.Vector2Operation.Add;
        [HutongGames.PlayMaker.Tooltip("The Vector2 result when it applies.")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 storeVector2Result;
        [HutongGames.PlayMaker.Tooltip("The float result when it applies")]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeFloatResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            this.vector1 = (FsmVector2) null;
            this.vector2 = (FsmVector2) null;
            this.operation = Vector2Operator.Vector2Operation.Add;
            this.storeVector2Result = (FsmVector2) null;
            this.storeFloatResult = (FsmFloat) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoVector2Operator();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoVector2Operator();

        private void DoVector2Operator()
        {
            Vector2 vector2_1 = this.vector1.Value;
            Vector2 vector2_2 = this.vector2.Value;
            switch (this.operation)
            {
                case Vector2Operator.Vector2Operation.DotProduct:
                    this.storeFloatResult.Value = Vector2.Dot(vector2_1, vector2_2);
                    break;
                case Vector2Operator.Vector2Operation.Distance:
                    this.storeFloatResult.Value = Vector2.Distance(vector2_1, vector2_2);
                    break;
                case Vector2Operator.Vector2Operation.Angle:
                    this.storeFloatResult.Value = Vector2.Angle(vector2_1, vector2_2);
                    break;
                case Vector2Operator.Vector2Operation.Add:
                    this.storeVector2Result.Value = vector2_1 + vector2_2;
                    break;
                case Vector2Operator.Vector2Operation.Subtract:
                    this.storeVector2Result.Value = vector2_1 - vector2_2;
                    break;
                case Vector2Operator.Vector2Operation.Multiply:
                    this.storeVector2Result.Value = Vector2.zero with
                    {
                        x = vector2_1.x * vector2_2.x,
                        y = vector2_1.y * vector2_2.y
                    };
                    break;
                case Vector2Operator.Vector2Operation.Divide:
                    this.storeVector2Result.Value = Vector2.zero with
                    {
                        x = vector2_1.x / vector2_2.x,
                        y = vector2_1.y / vector2_2.y
                    };
                    break;
                case Vector2Operator.Vector2Operation.Min:
                    this.storeVector2Result.Value = Vector2.Min(vector2_1, vector2_2);
                    break;
                case Vector2Operator.Vector2Operation.Max:
                    this.storeVector2Result.Value = Vector2.Max(vector2_1, vector2_2);
                    break;
            }
        }

        public enum Vector2Operation
        {
            DotProduct,
            Distance,
            Angle,
            Add,
            Subtract,
            Multiply,
            Divide,
            Min,
            Max,
        }
    }
}
