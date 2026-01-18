using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [HutongGames.PlayMaker.Tooltip("Smoothly Rotates a 2d Game Object so its right vector points at a Target. The target can be defined as a 2d Game Object or a 2d/3d world Position. If you specify both, then the position will be used as a local offset from the object's position.")]
    public class SmoothLookAt2d : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject to rotate to face a target.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("A target GameObject.")]
        public FsmGameObject targetObject;
        [HutongGames.PlayMaker.Tooltip("A target position. If a Target Object is defined, this is used as a local offset.")]
        public FsmVector2 targetPosition2d;
        [HutongGames.PlayMaker.Tooltip("A target position. If a Target Object is defined, this is used as a local offset.")]
        public FsmVector3 targetPosition;
        [HutongGames.PlayMaker.Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
        public FsmFloat rotationOffset;
        [HutongGames.PlayMaker.Tooltip("How fast the look at moves.")]
        [HasFloatSlider(0.5f, 15f)]
        public FsmFloat speed;
        [HutongGames.PlayMaker.Tooltip("Draw a line in the Scene View to the look at position.")]
        public FsmBool debug;
        [HutongGames.PlayMaker.Tooltip("If the angle to the target is less than this, send the Finish Event below. Measured in degrees.")]
        public FsmFloat finishTolerance;
        [HutongGames.PlayMaker.Tooltip("Event to send if the angle to target is less than the Finish Tolerance.")]
        public FsmEvent finishEvent;
        private GameObject previousGo;
        private Quaternion lastRotation;
        private Quaternion desiredRotation;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.targetObject = (FsmGameObject) null;
            FsmVector2 fsmVector2 = new FsmVector2();
            fsmVector2.UseVariable = true;
            this.targetPosition2d = fsmVector2;
            FsmVector3 fsmVector3 = new FsmVector3();
            fsmVector3.UseVariable = true;
            this.targetPosition = fsmVector3;
            this.rotationOffset = (FsmFloat) 0.0f;
            this.debug = (FsmBool) false;
            this.speed = (FsmFloat) 5f;
            this.finishTolerance = (FsmFloat) 1f;
            this.finishEvent = (FsmEvent) null;
        }

        public override void OnEnter() => this.previousGo = (GameObject) null;

        public override void OnLateUpdate() => this.DoSmoothLookAt();

        private void DoSmoothLookAt()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            GameObject gameObject = this.targetObject.Value;
            if ((Object) this.previousGo != (Object) ownerDefaultTarget)
            {
                this.lastRotation = ownerDefaultTarget.transform.rotation;
                this.desiredRotation = this.lastRotation;
                this.previousGo = ownerDefaultTarget;
            }
            Vector3 end = new Vector3(this.targetPosition2d.Value.x, this.targetPosition2d.Value.y, 0.0f);
            if (!this.targetPosition.IsNone)
                end += this.targetPosition.Value;
            if ((Object) gameObject != (Object) null)
            {
                end = gameObject.transform.position;
                Vector3 zero = Vector3.zero;
                if (!this.targetPosition.IsNone)
                    zero += this.targetPosition.Value;
                if (!this.targetPosition2d.IsNone)
                {
                    zero.x += this.targetPosition2d.Value.x;
                    zero.y += this.targetPosition2d.Value.y;
                }
                if (!this.targetPosition2d.IsNone || !this.targetPosition.IsNone)
                    end += gameObject.transform.TransformPoint((Vector3) this.targetPosition2d.Value);
            }
            Vector3 vector3 = end - ownerDefaultTarget.transform.position;
            vector3.Normalize();
            this.desiredRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(vector3.y, vector3.x) * 57.29578f - this.rotationOffset.Value);
            this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.speed.Value * Time.deltaTime);
            ownerDefaultTarget.transform.rotation = this.lastRotation;
            if (this.debug.Value)
                Debug.DrawLine(ownerDefaultTarget.transform.position, end, Color.grey);
            if (this.finishEvent == null || (double) Mathf.Abs(Vector3.Angle(this.desiredRotation.eulerAngles, this.lastRotation.eulerAngles)) > (double) this.finishTolerance.Value)
                return;
            this.Fsm.Event(this.finishEvent);
        }
    }
}
