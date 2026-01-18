using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [HutongGames.PlayMaker.Tooltip("Rotates a 2d Game Object on it's z axis so its forward vector points at a 2d or 3d position.")]
    public class LookAt2d : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject to rotate.")]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The 2d position to Look At.")]
        public FsmVector2 vector2Target;
        [HutongGames.PlayMaker.Tooltip("The 3d position to Look At. If not set to none, will be added to the 2d target")]
        public FsmVector3 vector3Target;
        [HutongGames.PlayMaker.Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
        public FsmFloat rotationOffset;
        [HutongGames.PlayMaker.Tooltip("Draw a debug line from the GameObject to the Target.")]
        [Title("Draw Debug Line")]
        public FsmBool debug;
        [HutongGames.PlayMaker.Tooltip("Color to use for the debug line.")]
        public FsmColor debugLineColor;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame = true;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.vector2Target = (FsmVector2) null;
            FsmVector3 fsmVector3 = new FsmVector3();
            fsmVector3.UseVariable = true;
            this.vector3Target = fsmVector3;
            this.debug = (FsmBool) false;
            this.debugLineColor = (FsmColor) Color.green;
            this.everyFrame = true;
        }

        public override void OnEnter()
        {
            this.DoLookAt();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoLookAt();

        private void DoLookAt()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            Vector3 end = new Vector3(this.vector2Target.Value.x, this.vector2Target.Value.y, 0.0f);
            if (!this.vector3Target.IsNone)
                end += this.vector3Target.Value;
            Vector3 vector3 = end - ownerDefaultTarget.transform.position;
            vector3.Normalize();
            float num = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
            ownerDefaultTarget.transform.rotation = Quaternion.Euler(0.0f, 0.0f, num - this.rotationOffset.Value);
            if (!this.debug.Value)
                return;
            Debug.DrawLine(ownerDefaultTarget.transform.position, end, this.debugLineColor.Value);
        }
    }
}
