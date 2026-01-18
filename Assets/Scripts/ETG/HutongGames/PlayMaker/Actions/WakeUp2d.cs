using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [HutongGames.PlayMaker.Tooltip("Forces a Game Object's Rigid Body 2D to wake up.")]
    public class WakeUp2d : ComponentAction<Rigidbody2D>
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject with a Rigidbody2d attached")]
        [CheckForComponent(typeof (Rigidbody2D))]
        [RequiredField]
        public FsmOwnerDefault gameObject;

        public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

        public override void OnEnter()
        {
            this.DoWakeUp();
            this.Finish();
        }

        private void DoWakeUp()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                return;
            this.rigidbody2d.WakeUp();
        }
    }
}
