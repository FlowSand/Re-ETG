#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Get the vector3 from a quaternion multiplied by a vector.")]
    [ActionCategory(ActionCategory.Quaternion)]
    public class GetQuaternionMultipliedByVector : QuaternionBaseAction
    {
        [Tooltip("The quaternion to multiply")]
        [RequiredField]
        public FsmQuaternion quaternion;
        [RequiredField]
        [Tooltip("The vector3 to multiply")]
        public FsmVector3 vector3;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("The resulting vector3")]
        public FsmVector3 result;

        public override void Reset()
        {
            this.quaternion = (FsmQuaternion) null;
            this.vector3 = (FsmVector3) null;
            this.result = (FsmVector3) null;
            this.everyFrame = false;
            this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }

        public override void OnEnter()
        {
            this.DoQuatMult();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate()
        {
            if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.Update)
                return;
            this.DoQuatMult();
        }

        public override void OnLateUpdate()
        {
            if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.LateUpdate)
                return;
            this.DoQuatMult();
        }

        public override void OnFixedUpdate()
        {
            if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.FixedUpdate)
                return;
            this.DoQuatMult();
        }

        private void DoQuatMult() => this.result.Value = this.quaternion.Value * this.vector3.Value;
    }
}
