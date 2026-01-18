using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Quaternion)]
    [HutongGames.PlayMaker.Tooltip("Creates a rotation that looks along forward with the the head upwards along upwards.")]
    public class QuaternionLookRotation : QuaternionBaseAction
    {
        [HutongGames.PlayMaker.Tooltip("the rotation direction")]
        [RequiredField]
        public FsmVector3 direction;
        [HutongGames.PlayMaker.Tooltip("The up direction")]
        public FsmVector3 upVector;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Store the inverse of the rotation variable.")]
        public FsmQuaternion result;

        public override void Reset()
        {
            this.direction = (FsmVector3) null;
            FsmVector3 fsmVector3 = new FsmVector3();
            fsmVector3.UseVariable = true;
            this.upVector = fsmVector3;
            this.result = (FsmQuaternion) null;
            this.everyFrame = true;
            this.everyFrameOption = QuaternionBaseAction.everyFrameOptions.Update;
        }

        public override void OnEnter()
        {
            this.DoQuatLookRotation();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate()
        {
            if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.Update)
                return;
            this.DoQuatLookRotation();
        }

        public override void OnLateUpdate()
        {
            if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.LateUpdate)
                return;
            this.DoQuatLookRotation();
        }

        public override void OnFixedUpdate()
        {
            if (this.everyFrameOption != QuaternionBaseAction.everyFrameOptions.FixedUpdate)
                return;
            this.DoQuatLookRotation();
        }

        private void DoQuatLookRotation()
        {
            if (this.upVector.IsNone)
                this.result.Value = Quaternion.LookRotation(this.direction.Value, this.upVector.Value);
            else
                this.result.Value = Quaternion.LookRotation(this.direction.Value);
        }
    }
}
