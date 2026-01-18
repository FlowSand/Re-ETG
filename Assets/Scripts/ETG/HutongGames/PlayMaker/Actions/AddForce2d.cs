using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Adds a 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis.")]
    [ActionCategory(ActionCategory.Physics2D)]
    public class AddForce2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField]
        [CheckForComponent(typeof (Rigidbody2D))]
        [HutongGames.PlayMaker.Tooltip("The GameObject to apply the force to.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Option for applying the force")]
        public ForceMode2D forceMode;
        [HutongGames.PlayMaker.Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollision2dInfo actions.")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 atPosition;
        [HutongGames.PlayMaker.Tooltip("A Vector2 force to add. Optionally override any axis with the X, Y parameters.")]
        [UIHint(UIHint.Variable)]
        public FsmVector2 vector;
        [HutongGames.PlayMaker.Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("A Vector3 force to add. z is ignored")]
        public FsmVector3 vector3;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            FsmVector2 fsmVector2 = new FsmVector2();
            fsmVector2.UseVariable = true;
            this.atPosition = fsmVector2;
            this.forceMode = ForceMode2D.Force;
            this.vector = (FsmVector2) null;
            FsmVector3 fsmVector3 = new FsmVector3();
            fsmVector3.UseVariable = true;
            this.vector3 = fsmVector3;
            FsmFloat fsmFloat1 = new FsmFloat();
            fsmFloat1.UseVariable = true;
            this.x = fsmFloat1;
            FsmFloat fsmFloat2 = new FsmFloat();
            fsmFloat2.UseVariable = true;
            this.y = fsmFloat2;
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
            Vector2 force = !this.vector.IsNone ? this.vector.Value : new Vector2(this.x.Value, this.y.Value);
            if (!this.vector3.IsNone)
            {
                force.x = this.vector3.Value.x;
                force.y = this.vector3.Value.y;
            }
            if (!this.x.IsNone)
                force.x = this.x.Value;
            if (!this.y.IsNone)
                force.y = this.y.Value;
            if (!this.atPosition.IsNone)
                this.rigidbody2d.AddForceAtPosition(force, this.atPosition.Value, this.forceMode);
            else
                this.rigidbody2d.AddForce(force, this.forceMode);
        }
    }
}
