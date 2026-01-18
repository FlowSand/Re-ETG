using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sets the 2d Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody 2D.")]
    [ActionCategory(ActionCategory.Physics2D)]
    public class SetVelocity2d : ComponentAction<Rigidbody2D>
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject with the Rigidbody2D attached")]
        [CheckForComponent(typeof (Rigidbody2D))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("A Vector2 value for the velocity")]
        public FsmVector2 vector;
        [HutongGames.PlayMaker.Tooltip("The y value of the velocity. Overrides 'Vector' x value if set")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("The y value of the velocity. Overrides 'Vector' y value if set")]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.vector = (FsmVector2) null;
            FsmFloat fsmFloat1 = new FsmFloat();
            fsmFloat1.UseVariable = true;
            this.x = fsmFloat1;
            FsmFloat fsmFloat2 = new FsmFloat();
            fsmFloat2.UseVariable = true;
            this.y = fsmFloat2;
            this.everyFrame = false;
        }

        public override void Awake() => this.Fsm.HandleFixedUpdate = true;

        public override void OnEnter()
        {
            this.DoSetVelocity();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnFixedUpdate()
        {
            this.DoSetVelocity();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        private void DoSetVelocity()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                return;
            Vector2 vector2 = !this.vector.IsNone ? this.vector.Value : this.rigidbody2d.velocity;
            if (!this.x.IsNone)
                vector2.x = this.x.Value;
            if (!this.y.IsNone)
                vector2.y = this.y.Value;
            this.rigidbody2d.velocity = vector2;
        }
    }
}
