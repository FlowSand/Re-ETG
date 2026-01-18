using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics)]
    [HutongGames.PlayMaker.Tooltip("Adds a force to a Game Object. Use Vector3 variable and/or Float variables for each axis.")]
    public class AddForce : ComponentAction<Rigidbody>
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject to apply the force to.")]
        [RequiredField]
        [CheckForComponent(typeof (Rigidbody))]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollisionInfo actions.")]
        public FsmVector3 atPosition;
        [HutongGames.PlayMaker.Tooltip("A Vector3 force to add. Optionally override any axis with the X, Y, Z parameters.")]
        [UIHint(UIHint.Variable)]
        public FsmVector3 vector;
        [HutongGames.PlayMaker.Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("Force along the Z axis. To leave unchanged, set to 'None'.")]
        public FsmFloat z;
        [HutongGames.PlayMaker.Tooltip("Apply the force in world or local space.")]
        public Space space;
        [HutongGames.PlayMaker.Tooltip("The type of force to apply. See Unity Physics docs.")]
        public ForceMode forceMode;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            FsmVector3 fsmVector3 = new FsmVector3();
            fsmVector3.UseVariable = true;
            this.atPosition = fsmVector3;
            this.vector = (FsmVector3) null;
            FsmFloat fsmFloat1 = new FsmFloat();
            fsmFloat1.UseVariable = true;
            this.x = fsmFloat1;
            FsmFloat fsmFloat2 = new FsmFloat();
            fsmFloat2.UseVariable = true;
            this.y = fsmFloat2;
            FsmFloat fsmFloat3 = new FsmFloat();
            fsmFloat3.UseVariable = true;
            this.z = fsmFloat3;
            this.space = Space.World;
            this.forceMode = ForceMode.Force;
            this.everyFrame = false;
        }

        public override void OnPreprocess() => this.Fsm.HandleFixedUpdate = true;

        public override void OnEnter()
        {
            this.DoAddForce();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnFixedUpdate() => this.DoAddForce();

        private void DoAddForce()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                return;
            Vector3 force = !this.vector.IsNone ? this.vector.Value : new Vector3();
            if (!this.x.IsNone)
                force.x = this.x.Value;
            if (!this.y.IsNone)
                force.y = this.y.Value;
            if (!this.z.IsNone)
                force.z = this.z.Value;
            if (this.space == Space.World)
            {
                if (!this.atPosition.IsNone)
                    this.rigidbody.AddForceAtPosition(force, this.atPosition.Value, this.forceMode);
                else
                    this.rigidbody.AddForce(force, this.forceMode);
            }
            else
                this.rigidbody.AddRelativeForce(force, this.forceMode);
        }
    }
}
