using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [HutongGames.PlayMaker.Tooltip("Is the rigidbody2D constrained from rotating?Note: Prefer SetRigidBody2dConstraints when working in Unity 5")]
    public class IsFixedAngle2d : ComponentAction<Rigidbody2D>
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject with the Rigidbody2D attached")]
        [CheckForComponent(typeof (Rigidbody2D))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Event sent if the Rigidbody2D does have fixed angle")]
        public FsmEvent trueEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if the Rigidbody2D doesn't have fixed angle")]
        public FsmEvent falseEvent;
        [HutongGames.PlayMaker.Tooltip("Store the fixedAngle flag")]
        [UIHint(UIHint.Variable)]
        public FsmBool store;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.trueEvent = (FsmEvent) null;
            this.falseEvent = (FsmEvent) null;
            this.store = (FsmBool) null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoIsFixedAngle();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoIsFixedAngle();

        private void DoIsFixedAngle()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                return;
            bool flag = (this.rigidbody2d.constraints & RigidbodyConstraints2D.FreezeRotation) != RigidbodyConstraints2D.None;
            this.store.Value = flag;
            this.Fsm.Event(!flag ? this.falseEvent : this.trueEvent);
        }
    }
}
