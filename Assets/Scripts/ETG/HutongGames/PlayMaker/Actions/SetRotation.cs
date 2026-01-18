using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sets the Rotation of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
    [ActionCategory(ActionCategory.Transform)]
    public class SetRotation : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject to rotate.")]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Use a stored quaternion, or vector angles below.")]
        [UIHint(UIHint.Variable)]
        public FsmQuaternion quaternion;
        [HutongGames.PlayMaker.Tooltip("Use euler angles stored in a Vector3 variable, and/or set each axis below.")]
        [Title("Euler Angles")]
        [UIHint(UIHint.Variable)]
        public FsmVector3 vector;
        public FsmFloat xAngle;
        public FsmFloat yAngle;
        public FsmFloat zAngle;
        [HutongGames.PlayMaker.Tooltip("Use local or world space.")]
        public Space space;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        [HutongGames.PlayMaker.Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
        public bool lateUpdate;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.quaternion = (FsmQuaternion) null;
            this.vector = (FsmVector3) null;
            FsmFloat fsmFloat1 = new FsmFloat();
            fsmFloat1.UseVariable = true;
            this.xAngle = fsmFloat1;
            FsmFloat fsmFloat2 = new FsmFloat();
            fsmFloat2.UseVariable = true;
            this.yAngle = fsmFloat2;
            FsmFloat fsmFloat3 = new FsmFloat();
            fsmFloat3.UseVariable = true;
            this.zAngle = fsmFloat3;
            this.space = Space.World;
            this.everyFrame = false;
            this.lateUpdate = false;
        }

        public override void OnEnter()
        {
            if (this.everyFrame || this.lateUpdate)
                return;
            this.DoSetRotation();
            this.Finish();
        }

        public override void OnUpdate()
        {
            if (this.lateUpdate)
                return;
            this.DoSetRotation();
        }

        public override void OnLateUpdate()
        {
            if (this.lateUpdate)
                this.DoSetRotation();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        private void DoSetRotation()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            Vector3 vector3 = this.quaternion.IsNone ? (this.vector.IsNone ? (this.space != Space.Self ? ownerDefaultTarget.transform.eulerAngles : ownerDefaultTarget.transform.localEulerAngles) : this.vector.Value) : this.quaternion.Value.eulerAngles;
            if (!this.xAngle.IsNone)
                vector3.x = this.xAngle.Value;
            if (!this.yAngle.IsNone)
                vector3.y = this.yAngle.Value;
            if (!this.zAngle.IsNone)
                vector3.z = this.zAngle.Value;
            if (this.space == Space.Self)
                ownerDefaultTarget.transform.localEulerAngles = vector3;
            else
                ownerDefaultTarget.transform.eulerAngles = vector3;
        }
    }
}
