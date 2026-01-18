using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Transforms 2d input into a 3d world space vector. E.g., can be used to transform input from a touch joystick to a movement vector.")]
    [NoActionTargets]
    [ActionCategory(ActionCategory.Input)]
    public class TransformInputToWorldSpace : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The horizontal input.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat horizontalInput;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("The vertical input.")]
        public FsmFloat verticalInput;
        [HutongGames.PlayMaker.Tooltip("Input axis are reported in the range -1 to 1, this multiplier lets you set a new range.")]
        public FsmFloat multiplier;
        [HutongGames.PlayMaker.Tooltip("The world plane to map the 2d input onto.")]
        [RequiredField]
        public TransformInputToWorldSpace.AxisPlane mapToPlane;
        [HutongGames.PlayMaker.Tooltip("Make the result relative to a GameObject, typically the main camera.")]
        public FsmGameObject relativeTo;
        [HutongGames.PlayMaker.Tooltip("Store the direction vector.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmVector3 storeVector;
        [HutongGames.PlayMaker.Tooltip("Store the length of the direction vector.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeMagnitude;

        public override void Reset()
        {
            this.horizontalInput = (FsmFloat) null;
            this.verticalInput = (FsmFloat) null;
            this.multiplier = (FsmFloat) 1f;
            this.mapToPlane = TransformInputToWorldSpace.AxisPlane.XZ;
            this.storeVector = (FsmVector3) null;
            this.storeMagnitude = (FsmFloat) null;
        }

        public override void OnUpdate()
        {
            Vector3 vector3_1 = new Vector3();
            Vector3 vector3_2 = new Vector3();
            if ((Object) this.relativeTo.Value == (Object) null)
            {
                switch (this.mapToPlane)
                {
                    case TransformInputToWorldSpace.AxisPlane.XZ:
                        vector3_1 = Vector3.forward;
                        vector3_2 = Vector3.right;
                        break;
                    case TransformInputToWorldSpace.AxisPlane.XY:
                        vector3_1 = Vector3.up;
                        vector3_2 = Vector3.right;
                        break;
                    case TransformInputToWorldSpace.AxisPlane.YZ:
                        vector3_1 = Vector3.up;
                        vector3_2 = Vector3.forward;
                        break;
                }
            }
            else
            {
                Transform transform = this.relativeTo.Value.transform;
                switch (this.mapToPlane)
                {
                    case TransformInputToWorldSpace.AxisPlane.XZ:
                        vector3_1 = transform.TransformDirection(Vector3.forward);
            vector3_1.y = 0.0f;
                        vector3_1 = vector3_1.normalized;
                        vector3_2 = new Vector3(vector3_1.z, 0.0f, -vector3_1.x);
                        break;
                    case TransformInputToWorldSpace.AxisPlane.XY:
                    case TransformInputToWorldSpace.AxisPlane.YZ:
                        vector3_1 = Vector3.up;
            vector3_1.z = 0.0f;
                        vector3_1 = vector3_1.normalized;
                        vector3_2 = transform.TransformDirection(Vector3.right);
                        break;
                }
            }
            float num1 = !this.horizontalInput.IsNone ? this.horizontalInput.Value : 0.0f;
            float num2 = !this.verticalInput.IsNone ? this.verticalInput.Value : 0.0f;
            Vector3 vector3_3 = (num1 * vector3_2 + num2 * vector3_1) * this.multiplier.Value;
            this.storeVector.Value = vector3_3;
            if (this.storeMagnitude.IsNone)
                return;
            this.storeMagnitude.Value = vector3_3.magnitude;
        }

        public enum AxisPlane
        {
            XZ,
            XY,
            YZ,
        }
    }
}
