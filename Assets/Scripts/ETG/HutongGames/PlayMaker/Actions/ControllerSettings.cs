using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Modify various character controller settings.\n'None' leaves the setting unchanged.")]
    [ActionCategory(ActionCategory.Character)]
    public class ControllerSettings : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject that owns the CharacterController.")]
        [CheckForComponent(typeof (CharacterController))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The height of the character's capsule.")]
        public FsmFloat height;
        [HutongGames.PlayMaker.Tooltip("The radius of the character's capsule.")]
        public FsmFloat radius;
        [HutongGames.PlayMaker.Tooltip("The character controllers slope limit in degrees.")]
        public FsmFloat slopeLimit;
        [HutongGames.PlayMaker.Tooltip("The character controllers step offset in meters.")]
        public FsmFloat stepOffset;
        [HutongGames.PlayMaker.Tooltip("The center of the character's capsule relative to the transform's position")]
        public FsmVector3 center;
        [HutongGames.PlayMaker.Tooltip("Should other rigidbodies or character controllers collide with this character controller (By default always enabled).")]
        public FsmBool detectCollisions;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;
        private GameObject previousGo;
        private CharacterController controller;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            FsmFloat fsmFloat1 = new FsmFloat();
            fsmFloat1.UseVariable = true;
            this.height = fsmFloat1;
            FsmFloat fsmFloat2 = new FsmFloat();
            fsmFloat2.UseVariable = true;
            this.radius = fsmFloat2;
            FsmFloat fsmFloat3 = new FsmFloat();
            fsmFloat3.UseVariable = true;
            this.slopeLimit = fsmFloat3;
            FsmFloat fsmFloat4 = new FsmFloat();
            fsmFloat4.UseVariable = true;
            this.stepOffset = fsmFloat4;
            FsmVector3 fsmVector3 = new FsmVector3();
            fsmVector3.UseVariable = true;
            this.center = fsmVector3;
            FsmBool fsmBool = new FsmBool();
            fsmBool.UseVariable = true;
            this.detectCollisions = fsmBool;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoControllerSettings();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoControllerSettings();

        private void DoControllerSettings()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            if ((Object) ownerDefaultTarget != (Object) this.previousGo)
            {
                this.controller = ownerDefaultTarget.GetComponent<CharacterController>();
                this.previousGo = ownerDefaultTarget;
            }
            if (!((Object) this.controller != (Object) null))
                return;
            if (!this.height.IsNone)
                this.controller.height = this.height.Value;
            if (!this.radius.IsNone)
                this.controller.radius = this.radius.Value;
            if (!this.slopeLimit.IsNone)
                this.controller.slopeLimit = this.slopeLimit.Value;
            if (!this.stepOffset.IsNone)
                this.controller.stepOffset = this.stepOffset.Value;
            if (!this.center.IsNone)
                this.controller.center = this.center.Value;
            if (this.detectCollisions.IsNone)
                return;
            this.controller.detectCollisions = this.detectCollisions.Value;
        }
    }
}
